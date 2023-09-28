// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;
using CommandLine.Text;
using ScanAnalyzer.A11yTestComparison;
using ScanCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScanAnalyzer
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
                Console.WriteLine($"Error: {caughtException}");
            }
        }

        static void RunWithParsedInputs(IOptions options)
        {
            Console.WriteLine($"Reading files from {options.InputDirectory}");
            Console.WriteLine($"Writing files to {options.OutputDirectory}");

            CreateCleanOutputDirectoryIfNeeded(options);
            FileCollection collection = FileCollection.Create(options.InputDirectory);
            A11yTestFileContent? previousFileContent = null;
            ErrorAggregator errorAggregator = new ErrorAggregator();

            foreach (string a11yTestFile in collection.FileList)
            {
                string a11yTestFileName = Path.GetFileName(a11yTestFile);
                DiffFlags diffFromPrevious = DiffFlags.Identical;
                A11yTestFileContent currentFileContent;
                try
                {
                    currentFileContent = A11yTestFileContent.Create(a11yTestFile);
                }
                catch (Exception)
                {
                    if (options.VerboseMode)
                    {
                        Console.WriteLine($"{a11yTestFileName} is malformed. Skippping this file.");
                    }
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
                    if (options.VerboseMode)
                    {
                        int errorCount = currentFileContent.ErrorAggregator.ErrorCount;
                        string inclusionReasion = (previousFileContent == null) ? "is the first valid file in the collection" : "differs from the previous file";
                        string suffix = errorCount == 0 ? string.Empty : $" ({errorCount} errors found)";
                        Console.WriteLine($"{a11yTestFileName} {inclusionReasion}.{suffix}");
                    }

                    errorAggregator.Merge(currentFileContent.ErrorAggregator);
                    previousFileContent = currentFileContent;
                }
            }

            if (options.VerboseMode)
            {
                errorAggregator.WriteSummaryByRuleDescription();
                errorAggregator.WriteSummaryByRuntimeId();
            }
            List<OutputFileInfo> outputFileInfos = TargetedA11yTestGenerator.GenerateTargetedA11yTestFiles(errorAggregator, options);
            WriteSummary(options, collection, outputFileInfos);
        }


        static void WriteSummary(IOptions options, FileCollection collection, List<OutputFileInfo> outputFileInfos)
        {
            Console.WriteLine("Axe-Windows scanner results:");
            Console.WriteLine($" {collection.FileList.Count} files were successfully read from {options.InputDirectory}");
            Console.WriteLine($" {outputFileInfos.Count} a11ytest files were written to {options.OutputDirectory}");
            Console.WriteLine($" Output file details:");
            foreach (OutputFileInfo info in outputFileInfos)
            {
                Console.WriteLine($"  {Path.GetFileName(info.OutputFileName)} was based on {Path.GetFileName(info.InputFileName)}");
                Console.WriteLine($"    {info.RuleViolations.Count} rule violation(s):");
                foreach (string ruleViolation in info.RuleViolations)
                {
                    Console.WriteLine($"      {ruleViolation}");
                }
            }
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
