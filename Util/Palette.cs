using System;

namespace Ethla.Util;

public class Palette<T> where T : notnull
{

    Dictionary<int, T> map1 = new Dictionary<int, T>();
    Dictionary<T, int> map2 = new Dictionary<T, int>();
    int nextId;

    public int IdFor(T t) => map2[t];

    public T FromId(int id) => map1[id];

    public void Add(T t)
    {
        int id = nextId++;
        map1[id] = t;
        map2[t] = id;
    }

    public int Count => nextId;

}
