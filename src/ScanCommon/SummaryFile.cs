// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.IO;

namespace ScanCommon
{
    /// <summary>
    /// A single class to manipulate the summary file. Shared between the Collector and the Analyzer
    /// </summary>
    public class SummaryFile
    {
        public static void WriteSummaryFile(string outputDirectory, SummaryData summaryData)
        {
            if (string.IsNullOrEmpty(summaryData.ScanId))
            {
                throw new Exception("ScanId cannot be null or empty");
            }
            string summaryFile = Path.Combine(outputDirectory, $"{summaryData.ScanId}.json");
            File.WriteAllText(summaryFile, JsonConvert.SerializeObject(summaryData));
        }

        public static SummaryData? ReadSummaryFile(string inputDirectory, string file)
        {
            string summaryFile = Path.Combine(inputDirectory, file);
            return JsonConvert.DeserializeObject<SummaryData>(File.ReadAllText(summaryFile));
        }
    }
}
