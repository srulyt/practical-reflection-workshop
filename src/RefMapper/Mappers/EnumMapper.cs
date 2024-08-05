using Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

internal class EnumMapper : IMapper
{
    private readonly Type _sourceType;
    private readonly Type _targetType;
    private IOrderedEnumerable<string> _sourceEnumValues;
    private IOrderedEnumerable<string> _targetEnumValues;

    public EnumMapper(Type sourceType, Type targetType)
    {
        _sourceType = sourceType;
        _targetType = targetType;

    }

    public void Activate(bool throwIfInvalid = false)
    {
        if (_targetType.IsEnum)
        {
            _sourceEnumValues = _sourceType.GetEnumNames().Order();
            _targetEnumValues = _targetType.GetEnumNames().Order();
            var haveSameValues = _sourceEnumValues.SequenceEqual(_targetEnumValues);
            IsValid = haveSameValues;
        }
        else
        {
            IsValid = false;
        }

        if (throwIfInvalid && !IsValid)
            throw new MappingTypeMismatchException(_sourceType, _targetType);
    }

    public bool IsValid { get; private set; }

    public object Map(object source, object target = null)
    {
        var currentName = Enum.GetName(_sourceType, source);
        var newValue = Enum.Parse(_targetType, currentName);
        return newValue;
    }
}