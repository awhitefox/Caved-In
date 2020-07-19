using System.Collections.Generic;

public static class IListExtensions
{
    public static T Pop<T>(this IList<T> list, int index)
    {
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }
}
