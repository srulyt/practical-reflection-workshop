namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

public class MappingTypeMismatchException : Exception
{
    public MappingTypeMismatchException(Type sourceType, Type targetType)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }

    public Type SourceType { get; }
    public Type TargetType { get; }
}
