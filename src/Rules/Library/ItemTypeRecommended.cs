// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.Resources;
using static Axe.Windows.Rules.PropertyConditions.ControlType;
using static Axe.Windows.Rules.PropertyConditions.Relationships;

namespace Axe.Windows.Rules.Library
{
    [RuleInfo(ID = RuleId.ItemTypeRecommended)]
    class ItemTypeRecommended : Rule
    {
        public ItemTypeRecommended()
        {
            Info.Description = Descriptions.ItemTypeRecommended;
            Info.HowToFix = HowToFix.ItemTypeRecommended;
            Info.Standard = A11yCriteriaId.ObjectInformation;
            Info.PropertyID = PropertyType.UIA_ItemTypePropertyId;
            Info.ErrorCode = EvaluationCode.NeedsReview;
        }

        public override bool PassesTest(IA11yElement e)
        {
            return false;
        }

        protected override Condition CreateCondition()
        {
            var recommendedControls = DataItem | ListItem | TreeItem;
            return recommendedControls & NoChild(HasSameType) & AnyChild(Image);
        }
    } // class
} // namespace
