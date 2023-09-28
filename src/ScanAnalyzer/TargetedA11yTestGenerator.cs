// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ScanAnalyzer.A11yTestComparison;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScanAnalyzer
{
    internal static class TargetedA11yTestGenerator
    {
        public static List<OutputFileInfo> GenerateTargetedA11yTestFiles(ErrorAggregator errorAggregator, IOptions options) 
        {
            List<OutputFileInfo> outputFileInfos = new List<OutputFileInfo>();

            string lastFirstLevelKey = string.Empty;
            string lastSecondLevelKey = string.Empty;
            OutputFileInfo? lastOutputFileInfo = null;

            long counter = 1;

            foreach(ListEntry<FileNameAndUniqueId> entry in errorAggregator.GetEntriesIndexedByRuntimeId())
            {
                if (entry.FirstLevelKey != lastFirstLevelKey)
                {
                    try
                    {
                        lastOutputFileInfo = CreateSingleTargetedA11yTestFile(entry, options, counter++);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Skipping {entry.Entry.FileName} due to error when processing it. Message = {e.Message}");
                        continue;
                    }
                    outputFileInfos.Add(lastOutputFileInfo);
                    lastFirstLevelKey = entry.FirstLevelKey;
                    lastSecondLevelKey = entry.SecondLevelKey;
                }
                else if (entry.SecondLevelKey != lastSecondLevelKey)
                {
                    lastOutputFileInfo?.AddRuleViolation(entry.SecondLevelKey);
                    lastSecondLevelKey = entry.SecondLevelKey;
                }
            }

            return outputFileInfos;
        }

        private static OutputFileInfo CreateSingleTargetedA11yTestFile(ListEntry<FileNameAndUniqueId> entry, IOptions options, long fileIndex)
        {
            string inputA11yTestFile = entry.Entry.FileName;
            string outputA11yTestFile = Path.Join(options.OutputDirectory, $"Axe-Windows-Scan-{fileIndex:d8}.a11ytest");

            A11yTestFileContent.CopyFileWithUpdatedMetadata(inputA11yTestFile, outputA11yTestFile, entry.Entry.UniqueId);

            return new OutputFileInfo(entry.SecondLevelKey, inputA11yTestFile, outputA11yTestFile);
        }
    }
}
