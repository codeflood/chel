namespace Chel.Abstractions.UnitTests
{
    public class TestMemberDescriptor : MemberDescriptor
    {
        public class Builder : MemberDescriptorBuilder<TestMemberDescriptor>
        {
            /*protected override TestMemberDescriptor CreateInstance()
            {
                return new TestMemberDescriptor();
            }*/
        }
    }
}