using System;
using System.Collections.Generic;

public static class RandomExtensions
{
    public static T PickFrom<T>(this Random random, IList<T> list)
    {
        return list[random.Next(list.Count)];
    }

    public static T PickFromParams<T>(this Random random, params T[] list) => PickFrom(random, list);

    public static T PopFrom<T>(this Random random, IList<T> list)
    {
        return list.Pop(random.Next(list.Count));
    }
}
