// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace ScanCollector
{
    public class Options : IOptions
    {
        [Option(Required = true, HelpText = "The name of the process to automatically scan")]
        public string ProcessName { get; set; } = string.Empty;

        [Option(Required = true, HelpText = "The directory where scans will be output")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Option(Required = false, HelpText = "The number of milliseconds between scans (default is 500)")]
        public uint MillisecondsBetweenScans { get; set; } = 500;

        [Option(Required = false, HelpText = "If specified, delete the existing output directory")]
        public bool OverwriteOutputDirectory { get; set; }
    }
}
