public class Tuple<T1, T2>
{
    public T1 Item1 { get; set; }
    public T2 Item2 { get; set; }
    public Tuple(T1 first, T2 second)
    {
        Item1 = first;
        Item2 = second;
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}