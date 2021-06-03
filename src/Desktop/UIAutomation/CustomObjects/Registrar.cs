// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Interop.UIAutomationCore;
using System;
using System.Collections.Generic;

namespace Axe.Windows.Desktop.UIAutomation.CustomObjects
{
    public class Registrar
    {
        class InternalPropertyDescription
        {
            public PropertyDescription PropertyDescription { get; }
            public int DynamicId { get; }

            public InternalPropertyDescription(int dynamicId, PropertyDescription propertyDescription)
            {
                DynamicId = dynamicId;
                PropertyDescription = propertyDescription;
            }
        }

        private CUIAutomationRegistrar _registrar = new CUIAutomationRegistrar();

        private readonly Dictionary<Guid, InternalPropertyDescription> _guidMap = new Dictionary<Guid, InternalPropertyDescription>();
        private readonly Dictionary<int, InternalPropertyDescription> _dynamicIdMap = new Dictionary<int, InternalPropertyDescription>();

        public int AddCustomProperty(PropertyDescription description)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));

            if (_guidMap.TryGetValue(description.Id, out InternalPropertyDescription internalPropertyDescription))
            {
                return internalPropertyDescription.DynamicId;
            }

            UIAutomationPropertyInfo info = new UIAutomationPropertyInfo
            {
                guid = description.Id,
                pProgrammaticName = description.Name,
                type = PropertyTypeConverter.FromPropertyType(description.Type)
            };
            _registrar.RegisterProperty(ref info, out int dynamicId);

            internalPropertyDescription = new InternalPropertyDescription(dynamicId, description);
            _guidMap.Add(description.Id, internalPropertyDescription);
            _dynamicIdMap.Add(dynamicId, internalPropertyDescription);

            return dynamicId;
        }
    }
}
