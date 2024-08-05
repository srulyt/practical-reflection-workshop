
using Microsoft.EE.ReflectionWorkshop.RefMapper.Tests.TestObjects.Collections;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Tests;

[TestClass]
public class CollectionsTests
{
    [TestMethod]
    public void MapObjectWithArrayTargetProperty()
    {
        var source = new SourceWithCollection { StringCollectionProperty = new[] { "String1", "String2", "String3" } };

        var target = new Mapper().Map<SourceWithCollection, TargetWithArray>(source);

        Assert.AreNotSame(source.StringCollectionProperty, target.StringCollectionProperty);
        CollectionAssert.AreEqual(source.StringCollectionProperty, target.StringCollectionProperty);
    }

    [TestMethod]
    public void MapObjectWithListTargetProperty()
    {
        var source = new SourceWithCollection { StringCollectionProperty = new[] { "String1", "String2", "String3" } };

        var target = new Mapper().Map<SourceWithCollection, TargetWithList>(source);

        CollectionAssert.AreEqual(source.StringCollectionProperty, target.StringCollectionProperty);
    }

    [TestMethod]
    public void MapObjectWithIEnumerableTargetProperty()
    {
        var source = new SourceWithCollection { StringCollectionProperty = new[] { "String1", "String2", "String3" } };

        var target = new Mapper().Map<SourceWithCollection, TargetWithIEnumerable>(source);

        Assert.AreNotSame(source.StringCollectionProperty, target.StringCollectionProperty);
        CollectionAssert.AreEqual(source.StringCollectionProperty, target.StringCollectionProperty.ToArray());
    }

    [TestMethod]
    public void MapItemsInCollection()
    {
        var source = new SimpleSourceCollection
        {
            Items = new[] {
                new SimpleSource {
                    BoolProperty = false,
                    IntProperty = 1,
                    StringProperty = "Item1"
                },
                new SimpleSource {
                    BoolProperty = true,
                    IntProperty = 2,
                    StringProperty = "Item2"
                },
                new SimpleSource {
                    BoolProperty = false,
                    IntProperty = 3,
                    StringProperty = "Item3"
                },
            }
        };

        var target = new Mapper().Map<SimpleSourceCollection, SimpleTargetCollection>(source);

        Assert.AreEqual(3, target.Items.Count());
        var firstItem = target.Items.Single(i => i.IntProperty == 1);
        var secondItem = target.Items.Single(i => i.IntProperty == 2);
        var thirdItem = target.Items.Single(i => i.IntProperty == 3);

        Assert.AreEqual("Item1", firstItem.StringProperty);
        Assert.AreEqual("Item2", secondItem.StringProperty);
        Assert.AreEqual("Item3", thirdItem.StringProperty);
    }

    [TestMethod]
    public void MapWithCollectionInConstructor()
    {
        var source = new RecordSourceWithStringCollection(new[] {"Item1", "Item2", "Item3"});

        var target = new Mapper().Map<RecordSourceWithStringCollection, RecordTargetWithStringCollection>(source);

        Assert.AreEqual(3, target.StringCollection.Count());
        
        Assert.AreEqual("Item1", target.StringCollection[0]);
        Assert.AreEqual("Item2", target.StringCollection[1]);
        Assert.AreEqual("Item3", target.StringCollection[2]);
    }

    [TestMethod]
    public void MapEnumCollection() {
        var source = new SourceEnumCollection {Items = new[] {SourceEnum.ValueOne, SourceEnum.ValueTwo, SourceEnum.ValueThree}};

        var target = new Mapper().Map<SourceEnumCollection, TargetEnumCollection>(source);

        Assert.AreEqual(3, target.Items.Count());

        Assert.AreEqual(TargetEnum.ValueOne, target.Items[0]);
        Assert.AreEqual(TargetEnum.ValueTwo, target.Items[1]);
        Assert.AreEqual(TargetEnum.ValueThree, target.Items[2]);
    }

    [TestMethod]
    public void ThrowMismatchExceptionWhenTypesDontMatch()
    {
        var source = new SourceWithCollection { StringCollectionProperty = new[] {"Item1", "Item2"} };
        var target = new SourceWithoutCollection();

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
        Assert.AreEqual(nameof(source.StringCollectionProperty), exception.PropertyName);
        Assert.AreEqual(typeof(string[]), exception.SourceType);
        Assert.AreEqual(typeof(string), exception.TargetType);
    }

    //null collection
}