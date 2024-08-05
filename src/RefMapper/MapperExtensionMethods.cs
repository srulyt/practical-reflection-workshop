namespace Microsoft.EE.ReflectionWorkshop.RefMapper;

public static class MapperExtensionMethods
{
    public static TTarget Map<TSource, TTarget>(this Mapper mapper, TSource source)
    {
        return (TTarget)mapper.Map(typeof(TSource), source, typeof(TTarget));
    }
}
