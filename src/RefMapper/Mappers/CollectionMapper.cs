using System.Collections;
using System.Reflection;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

internal class CollectionMapper : IMapper
{
    private readonly Type _sourceType;
    private readonly Type _targetType;
    private readonly Mapper _mapper;
    private Type _sourceEnumeratedType;
    private MethodInfo _enumerableCountMethod;
    private Type _targetEnumeratedType;
    private IMapper _enumeratedTypeMapper;

    public CollectionMapper(Type sourceType, Type targetType, Mapper mapper)
    {
        _sourceType = sourceType;
        _targetType = targetType;
        _mapper = mapper;
        _sourceEnumeratedType = sourceType.GetEnumeratedType();
        _enumerableCountMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Count" && m.GetParameters().Length == 1).MakeGenericMethod(new[] { _sourceEnumeratedType });
        _targetEnumeratedType = targetType.GetEnumeratedType();
    }

    public void Activate(bool throwIfInvalid = false)
    {
        _enumeratedTypeMapper = _mapper.GetMapper(_sourceEnumeratedType, _targetEnumeratedType);
        _enumeratedTypeMapper.Activate(throwIfInvalid);
        IsValid = _enumeratedTypeMapper.IsValid;
    }

    public bool IsValid { get; private set; }

    public object Map(object source, object target = null)
    {
        var length = (int)_enumerableCountMethod.Invoke(source, new[] { source });
        var targetArray = Array.CreateInstance(_targetEnumeratedType, length);

        var i = 0;
        var valueAsEnumerable = (IEnumerable)source;
        foreach (var item in valueAsEnumerable)
        {
            var targetItem = _enumeratedTypeMapper.Map(item);
            targetArray.SetValue(targetItem, i);
            i++;
        }

        object targetValue = targetArray;
        if (_targetType.IsGenericType && _targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var listType = typeof(List<>).MakeGenericType(_targetEnumeratedType);
            targetValue = Activator.CreateInstance(listType, new object[] { targetArray });
        }

        return targetValue;
    }
}