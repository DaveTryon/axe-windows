// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ScanCommon.A11yTestComparison
{
    public class ListEntry<T>
    {
        public string FirstLevelKey { get; }
        public string SecondLevelKey { get; }
        public T Entry { get; }

        public ListEntry(string firstLevelKey, string secondLevelKey, T entry)
        {
            FirstLevelKey = firstLevelKey;
            SecondLevelKey = secondLevelKey;
            Entry = entry;
        }
    }
}
