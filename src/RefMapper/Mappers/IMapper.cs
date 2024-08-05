namespace Microsoft.EE.ReflectionWorkshop.RefMapper.Mappers;

internal interface IMapper
{
    void Activate(bool throwIfInvalid = false);
    public bool IsValid {get;}
    object Map(object source, object target = null);
}
