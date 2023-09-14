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
    /// Class to help with error aggregation. It tracks failures in two lists:
    ///   Indexed by rule description, then by runtime ID, then by a11ytest file
    ///   Indexed by runtime Id, then by rule description, then by a11ytest file
    /// </summary>
    public class ErrorAggregator
    {
        private readonly TwoLevelDictionaryToListOfEntries<string> _indexedByDescriptionFirst = new TwoLevelDictionaryToListOfEntries<string>();
        private readonly TwoLevelDictionaryToListOfEntries<string> _indexedByRuntimeIdFirst = new TwoLevelDictionaryToListOfEntries<string>();

        public static ErrorAggregator CreateFromStream(Stream elementStream, string a11yTestFile)
        {
            ErrorAggregator errorAggregator = new ErrorAggregator();

            using (A11yElement rootElement = A11yElement.FromStream(elementStream))
            {
                errorAggregator.RecursivelyAddErrors(rootElement, a11yTestFile);
            }

            return errorAggregator;
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
                        _indexedByDescriptionFirst.AddError(ruleResult.Description, element.RuntimeId, a11yTestFile);
                        _indexedByRuntimeIdFirst.AddError(element.RuntimeId, ruleResult.Description, a11yTestFile);
                    }
                }
            }
        }

        public void WriteSummaryByRuleDescription()
        {
            string lastFirstLevelKey = string.Empty;
            string lastSecondLevelKey = string.Empty;

            Console.WriteLine("Contents by Rule Description");
            foreach (ListEntry<string> entry in _indexedByDescriptionFirst)
            {
                if (entry.FirstLevelKey != lastFirstLevelKey)
                {
                    Console.WriteLine($"  Rule: {entry.FirstLevelKey}");
                    lastFirstLevelKey = entry.FirstLevelKey;
                    lastSecondLevelKey = string.Empty;
                }

                if (entry.SecondLevelKey != lastSecondLevelKey)
                {
                    Console.WriteLine($"    {entry.SecondLevelKey}");
                    lastSecondLevelKey = entry.SecondLevelKey;
                }

                Console.WriteLine($"      {entry.Entry}");
            }
        }

        public void WriteSummaryByRuntimeId()
        {
            string lastFirstLevelKey = string.Empty;
            string lastSecondLevelKey = string.Empty;

            Console.WriteLine("Contents by Runtime ID");
            foreach (ListEntry<string> entry in _indexedByRuntimeIdFirst)
            {
                if (entry.FirstLevelKey != lastFirstLevelKey)
                {
                    Console.WriteLine($"  Runtime ID: {entry.FirstLevelKey}");
                    lastFirstLevelKey = entry.FirstLevelKey;
                    lastSecondLevelKey = string.Empty;
                }

                if (entry.SecondLevelKey != lastSecondLevelKey)
                {
                    Console.WriteLine($"    {entry.SecondLevelKey}");
                    lastSecondLevelKey = entry.SecondLevelKey;
                }

                Console.WriteLine($"      {entry.Entry}");
            }
        }

        public IEnumerable<ListEntry<string>> GetEntriesIndexedByRuntimeId()
        {
            foreach (ListEntry<string> entry in _indexedByRuntimeIdFirst)
            {
                yield return entry;
            }
        }

        /// <summary>
        /// Merges one ErrorDictioanry into another
        /// </summary>
        /// <param name="other">The ErrorDictionary being merged into this one</param>
        public void Merge(ErrorAggregator other)
        {
            _indexedByDescriptionFirst.Merge(other._indexedByDescriptionFirst);
            _indexedByRuntimeIdFirst.Merge(other._indexedByRuntimeIdFirst);
        }

        ///// <summary>
        ///// The ErrorDictionary is built with paths relative to the input directory. This updates the paths to be relative to the output directory
        ///// </summary>
        ///// <param name="outputDirectory">Where the output files are written</param>
        //public void UpdatePaths(string outputDirectory)
        //{
        //    foreach (string key in _errorLocations.Keys)
        //    {
        //        Dictionary<string, List<string>> idFileDictionary = _errorLocations[key];
        //        foreach (string runtimeId in idFileDictionary.Keys)
        //        {
        //            List<string> strings = idFileDictionary[runtimeId];
        //            for (int i = 0; i < strings.Count; i++)
        //            {
        //                string fileName = Path.GetFileName(strings[i]);
        //                string updatedPath = Path.Join(outputDirectory, fileName);
        //                strings[i] = updatedPath;
        //            }
        //        }
        //    }
        //}
    }
}
