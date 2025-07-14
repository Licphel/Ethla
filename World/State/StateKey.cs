namespace Ethla.World.State;

public interface StateKey
{

	string Key { get; }
	object InitValue { get; }
	object[] LegalVals { get; }

}

public class IntKey : StateKey
{

	readonly int init;

	IntKey(string key, int init, params int[] permit)
	{
		LegalVals = new object[permit.Length];
		for (int i = 0; i < permit.Length; i++) LegalVals[i] = permit[i];
		this.init = init;
		Key = key;
	}

	public object InitValue => init;

	public object[] LegalVals { get; }

	public string Key { get; }

	public static IntKey ByValues(string key, int init, params int[] permit)
	{
		return new IntKey(key, init, permit);
	}

	public static IntKey ByRange(string key, int init, int min, int max)
	{
		int[] arr = new int[max - min + 1];
		for (int i = min; i <= max; i++)
		{
			arr[i] = min + i;
		}

		return new IntKey(key, init, arr);
	}

}

public class BoolKey : StateKey
{

	static readonly object[] BOOL_ARR = { true, false };

	readonly bool init;

	public BoolKey(string key, bool init)
	{
		this.init = init;
		Key = key;
	}

	public object InitValue => init;

	public string Key { get; }

	public object[] LegalVals => BOOL_ARR;

}
