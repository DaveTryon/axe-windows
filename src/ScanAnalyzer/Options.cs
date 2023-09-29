// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace ScanAnalyzer
{
    public class Options : IOptions
    {
        [Option(Required = true, HelpText = "The input directory for AutoScan results")]
        public string InputDirectory { get; set; } = string.Empty;

        [Option(Required = true, HelpText = "The output directory for a11ytest files showing the highlighted items")]
        public string OutputDirectory { get; set; } = string.Empty;

        [Option(Required = false, HelpText = "If specified, delete the existing output directory")]
        public bool OverwriteOutputDirectory { get; set; }

        [Option(Required = false, HelpText = "If specified, runs in verbose mode")]
        public bool VerboseMode { get; set; }

        public void NormalizeInputs()
        {
            InputDirectory = System.IO.Path.GetFullPath(InputDirectory);
            OutputDirectory = System.IO.Path.GetFullPath(OutputDirectory);
        }
    }
}
