// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ScanCommon.A11yTestComparison;

namespace ScanAnalyzer
{
    internal static class TargetedA11yTestGenerator
    {
        public static void GenerateTargetedA11yTestFiles(ErrorAggregator errorAggregator, IOptions options) 
        {
            string lastRuntimeId = string.Empty;

            foreach(ListEntry<FileNameAndUniqueId> entry in errorAggregator.GetEntriesIndexedByRuntimeId())
            {
                if (entry.FirstLevelKey != lastRuntimeId)
                {
                    CreateSingleTargetedA11yTestFile(entry, options);
                    lastRuntimeId = entry.FirstLevelKey;
                }
            }
        }

        private static void CreateSingleTargetedA11yTestFile(ListEntry<FileNameAndUniqueId> entry, IOptions options)
        {
            // TODO
        }
    }
}
