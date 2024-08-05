namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Tests;

[TestClass]
public class CoreMappingTests
{
    [TestMethod]
    public void MapObject()
    {
        var source = new SimpleSource
        {
            StringProperty = "foo",
            IntProperty = 21,
            BoolProperty = true
        };
        var target = new SimpleTarget();

        new Mapper().Map(source, target);

        Assert.AreEqual(source.StringProperty, target.StringProperty);
        Assert.AreEqual(source.IntProperty, target.IntProperty);
        Assert.AreEqual(source.BoolProperty, target.BoolProperty);
    }

    [TestMethod]
    public void MapEnumByName()
    {
        var source = new EnumContainerSource { EnumProperty = SourceEnum.ValueThree };
        var target = new EnumContainerTarget();

        Assert.AreNotEqual(source.EnumProperty, target.EnumProperty);

        new Mapper().Map(source, target);

        Assert.AreEqual(TargetEnum.ValueThree, target.EnumProperty);
    }

    [TestMethod]
    public void ThrowMismatchExceptionWhenEnumsDontMatch()
    {
        var source = new EnumContainerSource { EnumProperty = SourceEnum.ValueTwo };
        var target = new MismatchEnumTarget();

        PropertyTypeMismatchException exception = null;
        try
        {
            new Mapper().Map(source, target);
        }
        catch (PropertyTypeMismatchException ex)
        {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.AreEqual(nameof(source.EnumProperty), exception.PropertyName);
        Assert.AreEqual(source.EnumProperty.GetType(), exception.SourceType);
        Assert.AreEqual(target.EnumProperty.GetType(), exception.TargetType);
    }

    [TestMethod]
    public void ThrowMismatchExceptionWhenTypesDontMatch()
    {
        var source = new MismatchTypeSource { TestProperty = 2.2 };
        var target = new MismatchTypeTarget();

        PropertyTypeMismatchException exception = null;
        try
        {
            new Mapper().Map(source, target);
        }
        catch (PropertyTypeMismatchException ex)
        {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.AreEqual(nameof(source.TestProperty), exception.PropertyName);
        Assert.AreEqual(source.TestProperty.GetType(), exception.SourceType);
        Assert.AreEqual(target.TestProperty.GetType(), exception.TargetType);
    }

    [TestMethod]
    public void ThrowPropertyMissingExceptionWhenMissingPropertiesOnTarget()
    {
        var source = new SimpleSource
        {
            StringProperty = "foo",
            IntProperty = 21,
            BoolProperty = true
        };
        var target = new MissingPropertiesTarget();

        PropertyMissingOnTargetException exception = null;
        try
        {
            new Mapper().Map(source, target);
        }
        catch (PropertyMissingOnTargetException ex)
        {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.AreEqual(nameof(source.BoolProperty), exception.PropertyName);
    }

    [TestMethod]
    private void CanMapShouldReturnTrueWhenCanMapObject()
    {
        var canMapResult = new Mapper().CanMap(typeof(SimpleSource), typeof(SimpleTarget));
        Assert.IsTrue(canMapResult);
    }

    [TestMethod]
    private void CanMapShouldReturnFalseWhenCantMapObject()
    {
        var cantMapResult = new Mapper().CanMap(typeof(MismatchTypeSource), typeof(MismatchTypeTarget));
        Assert.IsFalse(cantMapResult);
    }
}
