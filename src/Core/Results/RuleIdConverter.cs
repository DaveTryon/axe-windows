// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Axe.Windows.Core.Enums;
using Newtonsoft.Json;
using System;

namespace Axe.Windows.Core.Results
{
    public class RuleIdConverter : JsonConverter
    {
        private static readonly Type RuleIdType = typeof(RuleId);

        public override bool CanConvert(Type objectType)
        {
            return objectType == RuleIdType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == RuleIdType)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string value = (string)reader.Value;

                    if (Enum.TryParse<RuleId>(value, out RuleId ruleId))
                        return ruleId;
                }
                return RuleId.Indecisive;
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (RuleIdType == value.GetType())
            {
                RuleId ruleId = (RuleId)value;
                string ruleIdAsString = ruleId.ToString();
                writer.WriteValue(ruleIdAsString);
            }
        }
    }
}
