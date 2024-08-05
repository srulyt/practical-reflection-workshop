using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.EE.ReflectionWorkshop.RefMapper.Exceptions;

namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

internal class ObjectMapper : IMapper
{
    private readonly Type _sourceType;
    private readonly Type _targetType;
    private readonly Mapper _mapper;
    private readonly PropertyInfo[] _sourceProperties;
    private ConstructorMapping _constructor;
    private IEnumerable<PropertyInfo> _remainingProperties;
    private Dictionary<PropertyInfo, (PropertyInfo targetProperty, IMapper mapper)> _mappers;

    public ObjectMapper(Type sourceType, Type targetType, Mapper mapper)
    {
        _sourceType = sourceType;
        _targetType = targetType;
        _mapper = mapper;
        _sourceProperties = _sourceType.GetProperties();
    }

    public void Activate(bool throwIfInvalid = false)
    {
        _mappers = new Dictionary<PropertyInfo, (PropertyInfo targetProperty, IMapper mapper)>();
        var constructorMappings = _targetType.GetConstructors().Select(c =>
            {
                var mapping = new ConstructorMapping(c, _sourceProperties, _mapper);
                mapping.Evaluate();
                return mapping;
            });

        var validConstructors = constructorMappings.Where(m => m.IsValid);
        _constructor = validConstructors.OrderByDescending(m => m.ParameterCount).First();
        _remainingProperties = _sourceProperties.Except(_constructor.MappedProperties);
        
        var targetProperties = _targetType.GetProperties();
        foreach(var sourceProperty in _remainingProperties) {
            var targetProperty = targetProperties.SingleOrDefault(p => p.Name == sourceProperty.Name);
            if (targetProperty == null)
            {
                IsValid = false;
                if(throwIfInvalid)
                    throw new PropertyMissingOnTargetException(sourceProperty.Name);

                return;
            }

            var mapper = _mapper.GetMapper(sourceProperty.PropertyType, targetProperty.PropertyType);
            mapper.Activate();
            if(!mapper.IsValid)
            {
                IsValid = false;
                if(throwIfInvalid)
                    throw new PropertyTypeMismatchException(sourceProperty.Name, sourceProperty.PropertyType, targetProperty.PropertyType);

                return;
            }

            _mappers.Add(sourceProperty, new(targetProperty, mapper));
        }

        IsValid = true;
    }

    public bool IsValid {get; private set;}

    public object Map(object source, object target = null)
    {
        if(source == null)
            return null;

        if(target == null)
        {
            target = _constructor.Activate(source);
        }
        
        foreach (var sourceProperty in _remainingProperties)
        {
            var targetAndMapper = _mappers[sourceProperty];
            var sourceValue = sourceProperty.GetValue(source);
            var targetValue = targetAndMapper.targetProperty.GetValue(target);
            targetValue = targetAndMapper.mapper.Map(sourceValue, targetValue);
            
            targetAndMapper.targetProperty.SetValue(target, targetValue);
        }

        return target;
    }

    private class ConstructorMapping
    {
        private readonly ConstructorInfo _constructor;
        private readonly PropertyInfo[] _sourceProperties;
        private readonly Mapper _mapper;
        private readonly ParameterInfo[] _parameters;
        private Dictionary<PropertyInfo, IMapper> _mappers;

        public ConstructorMapping(ConstructorInfo constructor, PropertyInfo[] sourceProperties, Mapper mapper)
        {
            _constructor = constructor;
            _sourceProperties = sourceProperties;
            _mapper = mapper;
            _parameters = constructor.GetParameters();
            MappedProperties = new PropertyInfo[_parameters.Length];
        }

        public int ParameterCount { get { return _parameters.Length; } }
        public bool IsValid { get; private set; }
        public PropertyInfo[] MappedProperties { get; }

        public void Evaluate()
        {
            _mappers = new Dictionary<PropertyInfo, IMapper>();
            for (var i = 0; i < _parameters.Length; i++)
            {
                var matchingProperty = _sourceProperties.SingleOrDefault(p =>
                    String.Equals(_parameters[i].Name, p.Name, StringComparison.OrdinalIgnoreCase));
            
                if (matchingProperty == null)
                {
                    IsValid = false;
                    return;                    
                }
                
                var mapper = _mapper.GetMapper(matchingProperty.PropertyType, _parameters[i].ParameterType);
                mapper.Activate();
                if(!mapper.IsValid)
                {
                    IsValid = false;
                    return;
                }

                MappedProperties[i] = matchingProperty;
                _mappers.Add(matchingProperty, mapper);
            }

            IsValid = true;
        }

        public object Activate(object source)
        {
            var parameters = MappedProperties.Select((p, i) =>
            {
                var value = p.GetValue(source);
                var mapper = _mappers[p];
                value = mapper.Map(value);
                return value;
            }).ToArray();
            return _constructor.Invoke(parameters);
        }

    }
}
