// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ScanAnalyzer.A11yTestComparison
{
    public partial class TwoLevelDictionaryToListOfEntries<T> : IEnumerable<ListEntry<T>>
    {
        private readonly Dictionary<string, Dictionary<string, List<T>>> _firstLevelDictionary = new Dictionary<string, Dictionary<string, List<T>>>();

        /// <summary>
        /// Add a string to the dictionary, using the specified keys
        /// </summary>
        public void AddError(string firstLevelKey, string secondLevelKey, T entry)
        {
            if (!_firstLevelDictionary.TryGetValue(firstLevelKey, out Dictionary<string, List<T>>? secondLevelDictionary))
            {
                secondLevelDictionary = new Dictionary<string, List<T>>();
                _firstLevelDictionary.Add(firstLevelKey, secondLevelDictionary);
            }

            if (!secondLevelDictionary.TryGetValue(secondLevelKey, out List<T>? entryList))
            {
                entryList = new List<T>();
                secondLevelDictionary.Add(secondLevelKey, entryList);
            }

            entryList.Add(entry);
        }


        /// <summary>
        /// Merges another object into this one
        /// </summary>
        public void Merge(TwoLevelDictionaryToListOfEntries<T> other)
        {
            foreach (KeyValuePair<string, Dictionary<string, List<T>>> firstLevelPair in other._firstLevelDictionary)
            {
                if (!_firstLevelDictionary.TryGetValue(firstLevelPair.Key, out Dictionary<string, List<T>>? secondLevelDictionary))
                {
                    secondLevelDictionary = new Dictionary<string, List<T>>();
                    _firstLevelDictionary.Add(firstLevelPair.Key, secondLevelDictionary);
                }

                foreach (KeyValuePair<string, List<T>> secondLevelPair in firstLevelPair.Value)
                {
                    if (!secondLevelDictionary.TryGetValue(secondLevelPair.Key, out List<T>? entryList))
                    {
                        entryList = new List<T>();
                        secondLevelDictionary.Add(secondLevelPair.Key, entryList);
                    }

                    entryList.AddRange(secondLevelPair.Value);
                }
            }
        }

        public IEnumerator<ListEntry<T>> GetEnumerator()
        {
            foreach (KeyValuePair<string, Dictionary<string, List<T>>> firstLevelPair in _firstLevelDictionary)
            {
                foreach (KeyValuePair<string, List<T>> secondLevelPair in firstLevelPair.Value)
                {
                    foreach (T entry in secondLevelPair.Value)
                    {
                        yield return new ListEntry<T>(firstLevelPair.Key, secondLevelPair.Key, entry);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<ListEntry<T>> IEnumerable<ListEntry<T>>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
