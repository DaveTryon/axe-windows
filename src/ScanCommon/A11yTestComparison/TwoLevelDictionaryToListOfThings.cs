// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ScanCommon.A11yTestComparison
{
    public class TwoLevelDictionaryToListOfThings<T> : IEnumerable<TwoLevelDictionaryToListOfThings<T>.Entry<T>>
    {
        public class Entry<TEntry>
        {
            public string FirstLevelKey { get; }
            public string SecondLevelKey { get; }
            public TEntry Item { get; }

            public Entry(string firstLevelKey, string secondLevelKey, TEntry item)
            {
                FirstLevelKey = firstLevelKey;
                SecondLevelKey = secondLevelKey;
                Item = item;
            }
        }

        private readonly Dictionary<string, Dictionary<string, List<T>>> _firstLevelDictionary = new Dictionary<string, Dictionary<string, List<T>>>();

        /// <summary>
        /// Add a string to the dictionary, using the specified keys
        /// </summary>
        public void AddError(string firstLevelKey, string secondLevelKey, T item)
        {
            if (!_firstLevelDictionary.TryGetValue(firstLevelKey, out Dictionary<string, List<T>>? secondLevelDictionary))
            {
                secondLevelDictionary = new Dictionary<string, List<T>>();
                _firstLevelDictionary.Add(firstLevelKey, secondLevelDictionary);
            }

            if (!secondLevelDictionary.TryGetValue(secondLevelKey, out List<T>? itemList))
            {
                itemList = new List<T>();
                secondLevelDictionary.Add(secondLevelKey, itemList);
            }

            itemList.Add(item);
        }


        /// <summary>
        /// Merges another object into this one
        /// </summary>
        public void Merge(TwoLevelDictionaryToListOfThings<T> other)
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
                    if (!secondLevelDictionary.TryGetValue(secondLevelPair.Key, out List<T>? itemList))
                    {
                        itemList = new List<T>();
                        secondLevelDictionary.Add(secondLevelPair.Key, itemList);
                    }

                    itemList.AddRange(secondLevelPair.Value);
                }
            }
        }

        public IEnumerator<Entry<T>> GetEnumerator()
        {
            foreach (KeyValuePair<string, Dictionary<string, List<T>>> firstLevelPair in _firstLevelDictionary)
            {
                foreach (KeyValuePair<string, List<T>> secondLevelPair in firstLevelPair.Value)
                {
                    foreach (T item in secondLevelPair.Value)
                    {
                        yield return new Entry<T>(firstLevelPair.Key, secondLevelPair.Key, item);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<A11yTestComparison.TwoLevelDictionaryToListOfThings<T>.Entry<T>> IEnumerable<A11yTestComparison.TwoLevelDictionaryToListOfThings<T>.Entry<T>>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
