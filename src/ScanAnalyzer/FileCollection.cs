// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ScanCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScanAnalyzer
{
    internal class FileCollection
    {
        public IReadOnlyList<SummaryData> SummaryDataList { get; }

        private FileCollection(IReadOnlyList<SummaryData> summaryDataList)
        {
            SummaryDataList = summaryDataList;
        }

        public static FileCollection Create(string dataDirectory)
        {
            List<string> fileList = new List<string>();
            foreach (string file in Directory.EnumerateFiles(dataDirectory, "*.json"))
            {
                fileList.Add(file);
            }

            fileList.Sort(StringComparer.OrdinalIgnoreCase);

            List<SummaryData> summaryDataList = new List<SummaryData>();

            foreach (string file in fileList)
            {
                SummaryData? summaryData = SummaryFile.ReadSummaryFile(dataDirectory, file);
                if (summaryData == null)
                {
                    throw new Exception($"Malformed summary file: {file}");
                }
                summaryDataList.Add(summaryData);
            }

            return new FileCollection(summaryDataList);
        }
    }
}
