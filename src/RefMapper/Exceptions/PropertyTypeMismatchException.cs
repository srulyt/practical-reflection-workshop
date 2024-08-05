namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

public class PropertyTypeMismatchException : Exception
{
    public PropertyTypeMismatchException(string propertyName, Type sourceType, Type targetType)
    {
        PropertyName = propertyName;
        SourceType = sourceType;
        TargetType = targetType;
    }

    public string PropertyName { get; }
    public Type SourceType { get; }
    public Type TargetType { get; }
}
