using System;
using System.ComponentModel;
using Chel.Abstractions;
using Chel.Abstractions.Results;

namespace Chel.UnitTests.SampleCommands
{
    [Command("command")]
    public class ParameterTypesCommand : ICommand
    {
        [NamedParameter("bool", "value")]
        public bool Bool { get; set; }

        [NamedParameter("byte", "value")]
        public byte Byte { get; set; }

        [NamedParameter("bytearray", "value")]
        public byte[] ByteArray { get; set; }

        [NamedParameter("int", "value")]
        public int Int { get; set; }

        [NamedParameter("float", "value")]
        public float Float { get; set; }

        [NamedParameter("double", "value")]
        public double Double { get; set; }

        [NamedParameter("string", "value")]
        public string String { get; set; }

        [NamedParameter("date", "value")]
        public DateTime Date { get; set; }

        [NamedParameter("time", "value")]
        public TimeSpan Time { get; set; }

        [NamedParameter("guid", "value")]
        public Guid Guid { get; set; }

        [NamedParameter("guidarray", "value")]
        [TypeConverter(typeof(StringToGuidArrayTypeConverter))]
        public Guid[] GuidArray { get; set; }

        public CommandResult Execute()
        {
            throw new NotImplementedException();
        }
    }
}