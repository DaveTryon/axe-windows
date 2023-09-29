// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Automation;
using Axe.Windows.Automation.Data;
using CommandLine;
using CommandLine.Text;
using ScanCommon;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace ScanCollector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Exception? caughtException = null;
            bool parserError = false;

            try
            {
                using (var parser = CaseInsensitiveParser.Create())
                {
                    ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);
                    parserError = parserResult.Tag == ParserResultType.NotParsed;
                    parserResult.WithParsed(RunWithParsedInputs)
                        .WithNotParsed(_ =>
                        {
                            Console.WriteLine(HelpText.AutoBuild(parserResult));
                        });
                }
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                caughtException = e;
            }

            if (caughtException != null)
            {
                Console.WriteLine($"Error: {caughtException.Message}");
            }
        }

        static void RunWithParsedInputs(Options options)
        {
            options.NormalizeInputs();
            RunWithNormalizedInputs(options);
        }

        static void RunWithNormalizedInputs(IOptions options)
        {
            if (IsProcessRunning(options.ProcessName))
                throw new ArgumentException($"{options.ProcessName} is already running. Please close the process and try again.");

            CreateCleanOutputDirectory(options);

            int processId = WaitForProcessToStart(options.ProcessName);

            IScanner scanner = CreateScanner(processId, options);
            ScanUntilProcessEnds(scanner, processId, options);
        }

        static int WaitForProcessToStart(string processName)
        {
            Console.WriteLine($"Waiting for {processName} to start");
            while (!IsProcessRunning(processName))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
            int processId = Process.GetProcessesByName(processName).First().Id;

            Console.WriteLine($"{processName} started with process id {processId}");
            return processId;
        }

        static bool IsProcessRunning(int processId)
        {
            try
            {
                return !Process.GetProcessById(processId).HasExited;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        static void ScanUntilProcessEnds(IScanner scanner, int processId, IOptions options)
        {
            while (IsProcessRunning(processId))
            {
                string scanId = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

                Console.WriteLine($"Starting scan of {processId} with id of {scanId}.");
                FireAndForgetScan(scanner, scanId);
                Thread.Sleep(TimeSpan.FromMilliseconds(options.MillisecondsBetweenScans));
            }

            Console.WriteLine($"{processId} has exited. Shutting down.");
        }

        static async void FireAndForgetScan(IScanner scanner, string scanId)
        {
            try
            {
                ScanOptions scanOptions = new ScanOptions(scanId: scanId);
                ScanOutput scanOutput = await scanner.ScanAsync(scanOptions, CancellationToken.None);
                WindowScanOutput output = scanOutput.WindowScanOutputs.First();
            }
            catch (Exception)
            {
                Console.WriteLine($"Error in FireAndForgetScan");
            }
        }

        static IScanner CreateScanner(int processId, IOptions options)
        {
            Axe.Windows.Automation.Config config = Axe.Windows.Automation.Config.Builder
                .ForProcessId(processId)
                .WithAlwaysSaveTestFile()
                .WithOutputDirectory(options.OutputDirectory)
                .WithOutputFileFormat(OutputFileFormat.A11yTest)
                .Build();
            return ScannerFactory.CreateScanner(config);
        }

        static void CreateCleanOutputDirectory(IOptions options)
        {
            if (Directory.Exists(options.OutputDirectory))
            {
                if (options.OverwriteOutputDirectory)
                {
                    Console.WriteLine($"Overwriting existing output directory at {options.OutputDirectory}");
                    Directory.Delete(options.OutputDirectory, true);
                }
                else
                {
                    throw new ArgumentException($"{options.OutputDirectory} already exists. Please specify the --OverwriteOutputDirectory flag or use a different output directory.");
                }
            }

            Directory.CreateDirectory(options.OutputDirectory);
        }

        static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }
    }
}
