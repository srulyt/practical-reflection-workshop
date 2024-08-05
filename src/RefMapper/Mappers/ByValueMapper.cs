using Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

internal class ByValueMapper : IMapper
{
    private readonly Type _sourceType;
    private readonly Type _targetType;

    public ByValueMapper(Type sourceType, Type targetType)
    {
        _sourceType = sourceType;
        _targetType = targetType;
    }

    public void Activate(bool throwIfInvalid = false) {
        IsValid = _sourceType == _targetType;
        if(throwIfInvalid && !IsValid)
            throw new MappingTypeMismatchException(_sourceType, _targetType);
    }

    public bool IsValid {get; private set; }

    public object Map(object source, object target = null)
    {
        return source;
    }
}
