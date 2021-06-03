// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Axe.Windows.Desktop.UIAutomation.CustomObjects
{
    public class PropertyDescription
    {
        public Guid Id { get; }
        public string Name { get; }
        public PropertyType Type { get; }

        public PropertyDescription(Guid id, string name, PropertyType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
}
