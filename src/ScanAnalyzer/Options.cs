// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace ScanAnalyzer
{
    public class Options : IOptions
    {
        [Option(Required = true, HelpText = "The input directory for AutoScan results")]
        public string InputDirectory { get; set; } = string.Empty;

        [Option(Required = false, HelpText = "The output directory for filtered a11ytest files with errors")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Option(Required = false, HelpText = "If specified, delete the existing output directory")]
        public bool OverwriteOutputDirectory { get; set; }

        [Option(Required = false, HelpText = "Open files with errors as they are found")]
        public bool Interactive { get; set; }
    }
}
