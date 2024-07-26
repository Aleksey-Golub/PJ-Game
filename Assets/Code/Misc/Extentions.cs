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
}
