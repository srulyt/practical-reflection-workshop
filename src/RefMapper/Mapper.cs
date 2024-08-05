namespace Microsoft.EE.ReflectionWorkshop.RefMapper;

public class Mapper
{
    public bool CanMap(Type sourceType, Type targetType)
    {
        return true;
    }

    public void Map(object source, object target)
    {
        
    }

    public object Map(Type sourceType, object source, Type targetType)
    {
        return null;
    }

    public void Map(Type sourceType, object source, Type targetType, object target)
    {
        
    }
}
