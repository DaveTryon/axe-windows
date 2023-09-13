// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;
using CommandLine.Text;
using ScanCommon;
using ScanCommon.A11yTestComparison;
using System;
using System.Diagnostics;
using System.IO;

namespace ScanAnalyzer
{
    internal class Program
    {
        const int PROCESS_ERROR = -1;

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

        static void RunWithParsedInputs(IOptions options)
        {
            CreateCleanOutputDirectoryIfNeeded(options);
            FileCollection collection = FileCollection.Create(options.InputDirectory);
            A11yTestFileContent? previousFileContent = null;
            foreach (SummaryData summaryData in collection.SummaryDataList)
            {
                string a11yTestFile = summaryData.A11yTestFile;
                string a11yTestFileName = Path.GetFileName(a11yTestFile);
                DiffFlags diffFromPrevious = DiffFlags.Identical;
                A11yTestFileContent currentFileContent;
                try
                {
                    currentFileContent = A11yTestFileContent.Create(a11yTestFile);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"{a11yTestFileName} is malformed. Skippping this file.");
                    continue;
                }

                if (previousFileContent == null)
                {
                    diffFromPrevious = DiffFlags.All;
                }
                else
                {
                    diffFromPrevious = previousFileContent.Diff(currentFileContent);
                }

                if ((diffFromPrevious & DiffFlags.DifferenElementTree) != 0) // Ignore screenshot for now
                {
                    string inclusionReasion = (previousFileContent == null) ? "is the first valid file in the collection" : "differs from the previous file";
                    string suffix = summaryData.ErrorCount == 0 ? string.Empty : $" ({summaryData.ErrorCount} errors found)";
                    Console.WriteLine($"{a11yTestFileName} {inclusionReasion}.{suffix}");

                    if (summaryData.ErrorCount > 0 && !string.IsNullOrWhiteSpace(options.OutputDirectory))
                    {
                        File.Copy(summaryData.A11yTestFile, Path.Join(options.OutputDirectory, a11yTestFileName));
                    }
                    OptionallyLetUserViewTestFile(options, summaryData);
                    previousFileContent = currentFileContent;
                }
            }
        }

        private static void OptionallyLetUserViewTestFile(IOptions options, SummaryData summaryData)
        {
            if (options.Interactive && summaryData.ErrorCount > 0)
            {
                int processId = OpenFileInAccessibilityInsights(summaryData.A11yTestFile);
                if (processId == PROCESS_ERROR)
                {
                    Console.WriteLine($"Unable to open {summaryData.A11yTestFile} in Accessibility Insights. Please open it manually.");
                }
                else
                {
                    Process process = Process.GetProcessById(processId);
                    process.WaitForExit();
                }
            }
        }

        private static int OpenFileInAccessibilityInsights(string a11yTestFile)
        {
            string x86Path = Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Join(x86Path, "AccessibilityInsights", "1.1", "AccessibilityInsights.exe"),
                Arguments = $"\"{a11yTestFile}\"",
                UseShellExecute = true
            };

            Process? process = Process.Start(startInfo);
            return process == null ? PROCESS_ERROR : process.Id;
        }

        static void CreateCleanOutputDirectoryIfNeeded(IOptions options)
        {
            if (string.IsNullOrEmpty(options.OutputDirectory)) return;

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

    }
}
