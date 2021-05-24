using System;
namespace Nov.Caps.Int.D365.Crm.Core.OData
{
    public class ReferencePropertyAttribute : Attribute
    {
        public string PropertyName { get; }

        public ReferencePropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }
}
