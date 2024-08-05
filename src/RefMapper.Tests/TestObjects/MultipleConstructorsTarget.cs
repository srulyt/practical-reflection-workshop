namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Tests.TestObjects;

public class MultipleConstructorsTarget
{
    public MultipleConstructorsTarget(string stringProperty)
    {
        StringProperty = stringProperty + "first";
    }

    public MultipleConstructorsTarget(string stringProperty, int intProperty)
    {
        StringProperty = stringProperty + "second";
        IntProperty = intProperty;
    }

    public string StringProperty { get; set; }
    public int IntProperty { get; set; }
    public bool BoolProperty { get; set; }
}

