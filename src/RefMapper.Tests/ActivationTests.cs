namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Tests;

[TestClass]
public class ActivationTests
{
    [TestMethod]
    public void ActivateTarget()
    {
        var source = new SimpleSource
        {
            StringProperty = "foo",
            IntProperty = 21,
            BoolProperty = true
        };

        var target = new Mapper().Map<SimpleSource, SimpleTarget>(source);

        Assert.AreEqual(source.StringProperty, target.StringProperty);
        Assert.AreEqual(source.IntProperty, target.IntProperty);
        Assert.AreEqual(source.BoolProperty, target.BoolProperty);
    }

    [TestMethod]
    public void InstantiateRecord()
    {
        var source = new SimpleSource
        {
            StringProperty = "foo",
            IntProperty = 21,
            BoolProperty = true
        };

        var target = new Mapper().Map<SimpleSource, RecordTarget>(source);

        Assert.AreEqual(source.StringProperty, target.StringProperty);
        Assert.AreEqual(source.IntProperty, target.IntProperty);
        Assert.AreEqual(source.BoolProperty, target.BoolProperty);
    }

    [TestMethod]
    public void InstantiateRecordWithRecordParameter()
    {
        var source = new RootSource
        {
            DoubleProperty = 2.2,
            NestedObjectProperty = new SimpleSource
            {
                StringProperty = "foo",
                IntProperty = 21,
                BoolProperty = true
            }
        };

        var target = new Mapper().Map<RootSource, RootRecordTarget>(source);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.AreEqual(source.NestedObjectProperty.StringProperty, target.NestedObjectProperty.StringProperty);
        Assert.AreEqual(source.NestedObjectProperty.IntProperty, target.NestedObjectProperty.IntProperty);
        Assert.AreEqual(source.NestedObjectProperty.BoolProperty, target.NestedObjectProperty.BoolProperty);
    }

    [TestMethod]
    public void UseConstructorWithMostMatchingParameters()
    {
        var source = new SimpleSource
        {
            StringProperty = "foo",
            IntProperty = 21,
            BoolProperty = true
        };

        var target = new Mapper().Map<SimpleSource, MultipleConstructorsTarget>(source);

        Assert.AreEqual(source.StringProperty + "second", target.StringProperty);
        Assert.AreEqual(source.IntProperty, target.IntProperty);
        Assert.AreEqual(source.BoolProperty, target.BoolProperty);
    }

     [TestMethod]
    public void ActivateWithEnum()
    {
        var source = new EnumContainerRecordSource(SourceEnum.ValueThree);

        var target = new Mapper().Map<EnumContainerRecordSource, EnumContainerRecordTarget>(source);

        Assert.AreEqual(TargetEnum.ValueThree, target.EnumProperty);
    }
}
