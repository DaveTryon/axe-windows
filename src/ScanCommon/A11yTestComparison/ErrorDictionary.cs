// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Results;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScanCommon.A11yTestComparison
{
    /// <summary>
    /// Class to help with error aggregation. Failures are structured as follows:
    /// The outer dictionary is keyed by the rule description.
    /// The inner dictionary is keyed by the element runtime id.
    /// The inner dictionary values are the lsit of a11ytest files that contain this error instance
    /// </summary>
    public class ErrorDictionary
    {
        private Dictionary<string, Dictionary<string, List<string>>> _errorLocations = new Dictionary<string, Dictionary<string, List<string>>>();

        public static ErrorDictionary CreateFromStream(Stream elementStream, string a11yTestFile)
        {
            A11yElement rootElement = A11yElement.FromStream(elementStream);

            ErrorDictionary errorDictionary = new ErrorDictionary();

            errorDictionary.RecursivelyAddErrors(rootElement, a11yTestFile);

            return errorDictionary;
        }

        private void RecursivelyAddErrors(A11yElement element, string a11yTestFile)
        {
            foreach (A11yElement child in element.Children)
            {
                RecursivelyAddErrors(child, a11yTestFile);
            }

            if (element.ScanResults?.Items == null) return;

            foreach (ScanResult scanResult in element.ScanResults.Items)
            {
                foreach (RuleResult ruleResult in scanResult.Items)
                {
                    if (ruleResult.Status == ScanStatus.Fail && element.RuntimeId != null)
                    {
                        AddError(ruleResult.Description, element.RuntimeId, a11yTestFile);
                    }
                }
            }
        }

        private void AddError(string ruleDescription, string runtimeId, string a11ytestFile)
        {
            if (!_errorLocations.TryGetValue(ruleDescription, out Dictionary<string,List<string>>? idFileDictionary))
            {
                idFileDictionary = new Dictionary<string, List<string>>();
                _errorLocations.Add(ruleDescription, idFileDictionary);
            }

            if (!idFileDictionary.TryGetValue(runtimeId, out List<string>? a11yTestFileList))
            {
                a11yTestFileList = new List<string>();
                idFileDictionary.Add(runtimeId, a11yTestFileList);
            }

            a11yTestFileList.Add(a11ytestFile);
        }

        public void DumpContents()
        {
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in _errorLocations)
            {
                Console.WriteLine($"  Rule: {kvp.Key}");

                foreach (KeyValuePair<string, List<string>> kvpByRunTimeId in kvp.Value)
                {
                    Console.WriteLine($"    {kvpByRunTimeId.Key}");

                    foreach (string a11yTestFile in kvpByRunTimeId.Value)
                    {
                        Console.WriteLine($"        {a11yTestFile}");
                    }
                }
            }
        }

        /// <summary>
        /// Merges one ErrorDictioanry into another
        /// </summary>
        /// <param name="other">The ErrorDictionary being merged into this one</param>
        public void Merge(ErrorDictionary other)
        {
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp in other._errorLocations)
            {
                if (!_errorLocations.TryGetValue(kvp.Key, out Dictionary<string, List<string>>? idFileDictionary))
                {
                    idFileDictionary = new Dictionary<string, List<string>>();
                    _errorLocations.Add(kvp.Key, idFileDictionary);
                }

                foreach (KeyValuePair<string, List<string>> kvp2 in kvp.Value)
                {
                    if (!idFileDictionary.TryGetValue(kvp2.Key, out List<string>? a11yTestFileList))
                    {
                        a11yTestFileList = new List<string>();
                        idFileDictionary.Add(kvp2.Key, a11yTestFileList);
                    }

                    a11yTestFileList.AddRange(kvp2.Value);
                }
            }
        }

        /// <summary>
        /// The ErrorDictionary is built with paths relative to the input directory. This updates the paths to be relative to the output directory
        /// </summary>
        /// <param name="outputDirectory">Where the output files are written</param>
        public void UpdatePaths(string outputDirectory)
        {
            foreach (string key in _errorLocations.Keys)
            {
                Dictionary<string, List<string>> idFileDictionary = _errorLocations[key];
                foreach (string runtimeId in idFileDictionary.Keys)
                {
                    List<string> strings = idFileDictionary[runtimeId];
                    for (int i = 0; i < strings.Count; i++)
                    {
                        string fileName = Path.GetFileName(strings[i]);
                        string updatedPath = Path.Join(outputDirectory, fileName);
                        strings[i] = updatedPath;
                    }
                }
            }
        }
    }
}
