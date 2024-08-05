namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Tests;

[TestClass]
public class ObjectPropertiesMappingTests
{
    [TestMethod]
    public void MapWithObjectProperty()
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

        var target = new RootTarget
        {
            NestedObjectProperty = new SimpleTarget()
        };

        new Mapper().Map(source, target);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.AreEqual(source.NestedObjectProperty.StringProperty, target.NestedObjectProperty.StringProperty);
        Assert.AreEqual(source.NestedObjectProperty.IntProperty, target.NestedObjectProperty.IntProperty);
        Assert.AreEqual(source.NestedObjectProperty.BoolProperty, target.NestedObjectProperty.BoolProperty);
    }

    [TestMethod]
    public void MapNonActivatedObjectProperty()
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

        var target = new RootTarget();

        new Mapper().Map(source, target);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.AreEqual(source.NestedObjectProperty.StringProperty, target.NestedObjectProperty.StringProperty);
        Assert.AreEqual(source.NestedObjectProperty.IntProperty, target.NestedObjectProperty.IntProperty);
        Assert.AreEqual(source.NestedObjectProperty.BoolProperty, target.NestedObjectProperty.BoolProperty);
    }

    [TestMethod]
    public void MapNonActivatedRootObject()
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

        var target = new Mapper().Map<RootSource, RootTarget>(source);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.AreEqual(source.NestedObjectProperty.StringProperty, target.NestedObjectProperty.StringProperty);
        Assert.AreEqual(source.NestedObjectProperty.IntProperty, target.NestedObjectProperty.IntProperty);
        Assert.AreEqual(source.NestedObjectProperty.BoolProperty, target.NestedObjectProperty.BoolProperty);
    }

    [TestMethod]
    public void DeepCloneWhenMappingObjectsWithTheSameType()
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

        var target = new RootSource();

        new Mapper().Map(source, target);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.AreNotSame(source.NestedObjectProperty, target.NestedObjectProperty);
        Assert.AreEqual(source.NestedObjectProperty.StringProperty, target.NestedObjectProperty.StringProperty);
        Assert.AreEqual(source.NestedObjectProperty.IntProperty, target.NestedObjectProperty.IntProperty);
        Assert.AreEqual(source.NestedObjectProperty.BoolProperty, target.NestedObjectProperty.BoolProperty);
    }

    [TestMethod]
    public void MapNullObject()
    {
        var source = new RootSource
        {
            DoubleProperty = 2.2,
        };

        var target = new RootTarget();

        new Mapper().Map(source, target);

        Assert.AreEqual(source.DoubleProperty, target.DoubleProperty);
        Assert.IsNull(target.NestedObjectProperty);
    }
}
