// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ScanCommon
{
    /// <summary>
    /// Class to capture summary data from the scan and pass it into the analyzer
    /// </summary>
    public class SummaryData
    {
        /// <summary>
        /// The Id of this scan. This is a monotonically increasing number represented as a string
        /// </summary>
        public string ScanId { get; set; } = string.Empty;

        /// <summary>
        /// The A11yTest file that is associcated with this scan
        /// </summary>
        public string A11yTestFile { get; set; } = string.Empty;

        /// <summary>
        /// How many errors were found in the scan
        /// </summary>
        public int ErrorCount { get; set; }
    }
}
