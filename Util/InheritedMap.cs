namespace Ethla.Util;

public class InheritedMap
{

	private readonly Dictionary<Type, List<object>> dictionary = new Dictionary<Type, List<object>>();

	public void Add(object e, Type type = null)
	{
		Type t0 = type ?? e.GetType();
		Type basetype = t0.BaseType;
		if (!dictionary.ContainsKey(t0))
			dictionary[t0] = new List<object>();
		dictionary[t0].Add(e);
		if (basetype != null)
			Add(e, basetype);
		Type[] im = t0.GetInterfaces();
		foreach (Type t1 in im)
		{
			basetype = t0.BaseType;
			if (!dictionary.ContainsKey(t0))
				dictionary[t0] = new List<object>();
			dictionary[t0].Add(e);
			if (basetype != null)
				Add(e, basetype);
		}
	}

	public void Remove(object e, Type type = null)
	{
		Type t0 = type ?? e.GetType();
		Type basetype = t0.BaseType;
		if (dictionary.ContainsKey(t0))
			dictionary[t0].Remove(e);
		if (basetype != null)
			Add(e, basetype);
		Type[] im = t0.GetInterfaces();
		foreach (Type t1 in im)
		{
			basetype = t0.BaseType;
			if (dictionary.ContainsKey(t0))
				dictionary[t0].Remove(e);
			if (basetype != null)
				Add(e, basetype);
		}
	}

	public List<object> Get(Type type)
	{
		return dictionary.GetValueOrDefault(type) ?? new List<object>();
	}

}
