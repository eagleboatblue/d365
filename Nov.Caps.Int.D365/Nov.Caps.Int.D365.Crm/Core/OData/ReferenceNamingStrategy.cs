using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nov.Caps.Int.D365.Crm.Core.OData
{
    public class ReferenceNamingStrategy : DefaultNamingStrategy
    {
        private readonly Type valueType;
        private readonly Dictionary<string, string> references;

        public ReferenceNamingStrategy(Type valueType)
        {
            this.valueType = valueType;
            this.references = new Dictionary<string, string>();

            foreach (var prop in valueType.GetProperties())
            {
                var refAttrs = prop.GetCustomAttributes(typeof(JsonConverterAttribute), false);

                if (!refAttrs.Any())
                {
                    continue;
                }

                var refAttr = (refAttrs.First() as JsonConverterAttribute);

                if (!refAttr.ConverterType.Equals(typeof(ReferenceValueConverter)))
                {
                    continue;
                }

                var deserAttrs = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false);
                string deserName;

                if (deserAttrs.Any())
                {
                    deserName = (deserAttrs.First() as JsonPropertyAttribute).PropertyName;
                }
                else
                {
                    deserName = prop.Name;
                }

                var serAttrs = prop.GetCustomAttributes(typeof(ReferencePropertyAttribute), false);
                string serName;

                if (serAttrs.Any())
                {
                    serName = (serAttrs.First() as ReferencePropertyAttribute).PropertyName;
                }
                else
                {
                    serName = prop.Name;
                }

                this.references.Add(deserName, serName);
            }
        }

        public override string GetPropertyName(string name, bool hasSpecifiedName)
        {
            if (this.references.ContainsKey(name))
            {
                var serName = this.references[name];

                return $"{serName}@odata.bind";
            }

            return base.GetPropertyName(name, hasSpecifiedName);
        }
    }
}
