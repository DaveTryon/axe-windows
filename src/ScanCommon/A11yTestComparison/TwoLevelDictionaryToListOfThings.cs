// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ScanCommon.A11yTestComparison
{
    public class TwoLevelDictionaryToListOfThings : IEnumerable<TwoLevelDictionaryToListOfThings.Entry>
    {
        public class Entry
        {
            public string FirstLevelKey { get; }
            public string SecondLevelKey { get; }
            public string Item { get; }

            public Entry(string firstLevelKey, string secondLevelKey, string item)
            {
                FirstLevelKey = firstLevelKey;
                SecondLevelKey = secondLevelKey;
                Item = item;
            }
        }

        private readonly Dictionary<string, Dictionary<string, List<string>>> _firstLevelDictionary = new Dictionary<string, Dictionary<string, List<string>>>();

        /// <summary>
        /// Add a string to the dictionary, using the specified keys
        /// </summary>
        public void AddError(string firstLevelKey, string secondLevelKey, string item)
        {
            if (!_firstLevelDictionary.TryGetValue(firstLevelKey, out Dictionary<string, List<string>>? secondLevelDictionary))
            {
                secondLevelDictionary = new Dictionary<string, List<string>>();
                _firstLevelDictionary.Add(firstLevelKey, secondLevelDictionary);
            }

            if (!secondLevelDictionary.TryGetValue(secondLevelKey, out List<string>? stringList))
            {
                stringList = new List<string>();
                secondLevelDictionary.Add(secondLevelKey, stringList);
            }

            stringList.Add(item);
        }


        /// <summary>
        /// Merges another object into this one
        /// </summary>
        public void Merge(TwoLevelDictionaryToListOfThings other)
        {
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> firstLevelPair in other._firstLevelDictionary)
            {
                if (!_firstLevelDictionary.TryGetValue(firstLevelPair.Key, out Dictionary<string, List<string>>? secondLevelDictionary))
                {
                    secondLevelDictionary = new Dictionary<string, List<string>>();
                    _firstLevelDictionary.Add(firstLevelPair.Key, secondLevelDictionary);
                }

                foreach (KeyValuePair<string, List<string>> secondLevelPair in firstLevelPair.Value)
                {
                    if (!secondLevelDictionary.TryGetValue(secondLevelPair.Key, out List<string>? stringList))
                    {
                        stringList = new List<string>();
                        secondLevelDictionary.Add(secondLevelPair.Key, stringList);
                    }

                    stringList.AddRange(secondLevelPair.Value);
                }
            }
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> firstLevelPair in _firstLevelDictionary)
            {
                foreach (KeyValuePair<string, List<string>> secondLevelPair in firstLevelPair.Value)
                {
                    foreach (string item in secondLevelPair.Value)
                    {
                        yield return new Entry(firstLevelPair.Key, secondLevelPair.Key, item);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
