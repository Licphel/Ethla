using Spectrum.Utils;

namespace Ethla.World.State;

public interface StateMap
{

	dynamic GetState(StateKey key);

	dynamic SetState(StateKey key, object value);

}

public interface FlatStateMap : StateMap
{

	Dictionary<StateKey, object> ToMap();

}

public class StateContainer : FlatStateMap
{

	public HashSet<StateKey> PermittedKeys = new HashSet<StateKey>();
	public int Meta;
	public StateDefinition Definition;

	Dictionary<StateKey, object> states = new Dictionary<StateKey, object>();
	int Hash;

	public dynamic GetState(StateKey key)
	{
		if (!PermittedKeys.Contains(key))
		{
			Logger.Warn("Try get a not permitted state!");
			return default;
		}
		return states.GetValueOrDefault(key, key.InitValue);
	}

	public dynamic SetState(StateKey key, object value)
	{
		if (!PermittedKeys.Contains(key))
		{
			Logger.Warn("Try set a not permitted state!");
			return default;
		}

		return SetUnchecked(key, value);
	}

	public Dictionary<StateKey, object> ToMap()
	{
		return states;
	}

	public void Initialize(StateDefinition initializer)
	{
		Definition = initializer;
		if (PermittedKeys.Count != 0)
		{
			Logger.Warn("Cannot fill a not empty state map.");
			return;
		}
		foreach (StateKey o in initializer)
		{
			PermittedKeys.Add(o);
		}
		foreach (StateKey key in PermittedKeys)
		{
			states[key] = key.InitValue;//Fill default values, for hashCode.
		}
		RecalcHash();
	}

	public dynamic SetUnchecked(StateKey key, object value)
	{
		dynamic prev = states.GetValueOrDefault(key, key.InitValue);
		states[key] = value;
		RecalcHash();
		return prev;
	}

	public void Copy(StateContainer mapper)
	{
		foreach (KeyValuePair<StateKey, object> kv in mapper.ToMap())
		{
			SetState(kv.Key, kv.Value);
		}
		RecalcHash();
	}

	public void RecalcHash()
	{
		Hash = 0;
		foreach (KeyValuePair<StateKey, object> kv in states)
		{
			Hash += kv.Key.GetHashCode();
			Hash -= kv.Value.GetHashCode();
		}
		Meta = Definition.IdFor(this);
	}

	public override bool Equals(object obj)
	{
		if (obj is StateContainer container)
		{
			if (Hash != container.Hash)
			{
				return false;
			}

			return states.SequenceEqual(container.states);
		}

		return false;
	}

	public override int GetHashCode()
	{
		return Hash;
	}

}
