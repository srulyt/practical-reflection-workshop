using System.Diagnostics.Contracts;
using System.Reflection;
using Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper;

public class Mapper
{
    public bool CanMap(Type sourceType, Type targetType)
    {
        return true;
    }

    public void Map(object source, object target)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        Map(sourceType, source, targetType, target);
    }

    public object Map(Type sourceType, object source, Type targetType)
    {
        object target = Activate(targetType);
        Map(sourceType, source, targetType, target);
        return target;
    }

    private object Activate(Type targetType)
    {
        return Activator.CreateInstance(targetType);
    }

    public void Map(Type sourceType, object source, Type targetType, object target)
    {
        var sourceProperties = sourceType.GetProperties();
        var targetProperties = targetType.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var targetProperty = targetProperties.SingleOrDefault(p => p.Name == sourceProperty.Name);
            if (targetProperty == null)
                throw new PropertyMissingOnTargetException(sourceProperty.Name);

            var setter = GetSetter(sourceProperty, targetProperty);
            setter(target, sourceProperty.GetValue(source));
        }
    }

    private Action<object, object> GetSetter(PropertyInfo sourceProperty, PropertyInfo targetProperty)
    {
        if (sourceProperty.PropertyType.IsEnum && targetProperty.PropertyType.IsEnum)
        {
            return GetEnumSetter(sourceProperty, targetProperty);
        }

        if (sourceProperty.PropertyType.IsValueType || sourceProperty.PropertyType == typeof(string))
        {
            if (sourceProperty.PropertyType != targetProperty.PropertyType)
            {
                throw new PropertyTypeMismatchException(sourceProperty.Name, sourceProperty.PropertyType, targetProperty.PropertyType);
            }

            return targetProperty.SetValue;
        }

        return GetObjectSetter(sourceProperty, targetProperty);
    }

    private Action<object, object> GetEnumSetter(PropertyInfo sourceProperty, PropertyInfo targetProperty)
    {
        var sourceEnumValues = sourceProperty.PropertyType.GetEnumNames().Order();
        var targetEnumValues = targetProperty.PropertyType.GetEnumNames().Order();
        var haveSameValues = sourceEnumValues.SequenceEqual(targetEnumValues);

        if (!haveSameValues)
        {
            throw new PropertyTypeMismatchException(sourceProperty.Name, sourceProperty.PropertyType, targetProperty.PropertyType);
        }

        return (object obj, object value) =>
        {
            var currentName = Enum.GetName(sourceProperty.PropertyType, value);
            var newValue = Enum.Parse(targetProperty.PropertyType, currentName);
            targetProperty.SetValue(obj, newValue);
        };
    }

    private Action<object, object> GetObjectSetter(PropertyInfo sourceProperty, PropertyInfo targetProperty)
    {
        return (object target, object value) =>
        {
            var targetValue = targetProperty.GetValue(target);
            if (targetValue is null)
            {
                targetValue = Activate(targetProperty.PropertyType);
            }

            Map(sourceProperty.PropertyType, value, targetProperty.PropertyType, targetValue);
            targetProperty.SetValue(target, targetValue);
        };
    }
}
