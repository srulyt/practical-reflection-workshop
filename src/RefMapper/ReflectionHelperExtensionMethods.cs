using System.Reflection;

internal static class ReflectionHelperExtensionMethods
{
    public static bool DoesNotRequireMapping(this Type type)
    {
        return type.IsValueType || type == typeof(string);
    }

    public static Type GetEnumeratedType(this Type enumerableType)
    {
        var iEnumerableType = enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? enumerableType 
            : enumerableType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        var enumeratedType = iEnumerableType.GetGenericArguments()[0];
        return enumeratedType;
    }
}