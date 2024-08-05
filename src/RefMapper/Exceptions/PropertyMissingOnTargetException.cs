namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

public class PropertyMissingOnTargetException : Exception
{
    public PropertyMissingOnTargetException(string propertyName)
    {
        PropertyName = propertyName;
    }

    public string PropertyName { get; }

}