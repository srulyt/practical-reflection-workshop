using System.Collections;
using Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper;

public class Mapper
{
    public bool CanMap(Type sourceType, Type targetType)
    {
        var mapper = GetMapper(sourceType, targetType);
        mapper.Activate();
        return mapper.IsValid;
    }

    public void Map(object source, object target)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        Map(sourceType, source, targetType, target);
    }

    public object Map(Type sourceType, object source, Type targetType)
    {
        var mapper = GetMapper(sourceType, targetType);
        mapper.Activate(true);
        return mapper.Map(source);
    }

    public void Map(Type sourceType, object source, Type targetType, object target)
    {
        var mapper = GetMapper(sourceType, targetType);
        mapper.Activate(true);
        mapper.Map(source, target);
    }

    internal IMapper GetMapper(Type sourceType, Type targetType) {
         if (sourceType.IsEnum)
        {
            return new EnumMapper(sourceType, targetType);
        }

        if (sourceType.DoesNotRequireMapping())
        {
            return new ByValueMapper(sourceType, targetType);
        }

        if (sourceType.IsAssignableTo(typeof(IEnumerable)))
        {
            return new CollectionMapper(sourceType, targetType, this);
        }

        return new ObjectMapper(sourceType, targetType, this);
    }
}
