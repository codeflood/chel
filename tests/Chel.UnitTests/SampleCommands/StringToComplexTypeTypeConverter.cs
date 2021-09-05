using System;
using System.ComponentModel;
using System.Globalization;

namespace Chel.UnitTests.SampleCommands
{
    public class StringToComplexTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var elements = value.ToString().Split(":", StringSplitOptions.RemoveEmptyEntries);
            if(elements.Length != 2)
                throw new InvalidOperationException("ComplexType requires 2 elements.");

            return new ComplexType
            {
                Name = elements[0],
                Number = int.Parse(elements[1])
            };
        }
    }
}