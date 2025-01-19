using System;
using System.Collections.Generic;

public static class Extentions
{
    /// <summary>
    /// Set all items to default value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Refresh<T>(this T[] array) where T : class
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = default(T);
        }
    }

    /// <summary>
    /// Get next element of list. Return next element if list contains current, otherwise return first element. Return first if current is last.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public static T NextCircular<T>(this IList<T> list, T current)
    {
        int index = list.IndexOf(current);

        if (index < 0)
            return list[0];

        int nextIndex = index + 1;
        return nextIndex < list.Count ? list[nextIndex] : list[0];
    }

    /// <summary>
    /// Get previous element of list. Return previous element if list contains current, otherwise return first element. Return last if current is first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public static T PreviousCircular<T>(this IList<T> list, T current)
    {
        int index = list.IndexOf(current);

        if (index < 0)
            return list[0];

        int prevIndex = index - 1;
        return prevIndex >= 0 ? list[prevIndex] : list[^1];
    }

    /// <summary>
    /// Convert time in seconds to formated string.
    /// Find more formats <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings">here</see>
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ToStringFormated(this float seconds, string format = "mm':'ss")
    {
        return TimeSpan.FromSeconds(seconds).ToString(format);
    }

    public static string ToStringDynamicFormated(this float seconds)
    {
        string format = string.Empty;
        if (seconds / (60 * 60 * 24) >= 1)
            format = "%d':'hh':'mm':'ss";
        else if (seconds / (60 * 60) >= 1)
            format = "%h':'mm':'ss";
        else if (seconds / (60) >= 1)
            format = "%m':'ss";
        else
            format = "ss";

        return TimeSpan.FromSeconds(seconds).ToString(format);
    }
}
