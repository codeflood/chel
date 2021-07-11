using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Chel.UnitTests.SampleCommands
{
    public class StringToGuidArrayTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var output = new List<Guid>();
            var ids = value.ToString().Split("|", StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var id in ids)
            {
                output.Add(Guid.Parse(id));
            }

            return output.ToArray();
        }
    }
}