// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace ScanAnalyzer
{
    internal class OutputFileInfo
    {
        public List<string> RuleViolations { get; }
        public string InputFileName { get; }
        public string OutputFileName { get; }

        public OutputFileInfo(string ruleViolation, string inputFileName, string outputFileName)
        {
            InputFileName = inputFileName;
            OutputFileName = outputFileName;
            RuleViolations = new List<string> { ruleViolation };
        }

        public void AddRuleViolation(string ruleViolation)
        {
            RuleViolations.Add(ruleViolation);
        }
    }
}
