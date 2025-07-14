using Ethla.Util;
using Spectrum.Maths;

namespace Ethla.World.State;

public class StateDefinition : HashSet<StateKey>
{

	public readonly Palette<StateContainer> Containers = new Palette<StateContainer>();
	public readonly List<StateContainer> ValidStates = new List<StateContainer>();
	public int StateCount => ValidStates.Count;

	public void CollectStates()
	{
		List<List<Tuple<StateKey, object>>> list = new List<List<Tuple<StateKey, object>>>();

		foreach (StateKey key in this)
		{
			List<Tuple<StateKey, object>> values = new List<Tuple<StateKey, object>>();

			foreach (object o in key.LegalVals)
			{
				values.Add(Tuple.Create(key, o));
			}
			list.Add(values);
		}

		list = Cartesian.GetDot(list);

		if (list.Count == 0)
		{
			List<Tuple<StateKey, object>> values = new List<Tuple<StateKey, object>>();
			list.Add(values);
		}

		foreach (List<Tuple<StateKey, object>> listIn in list)
		{
			StateContainer mapper = new StateContainer();
			mapper.Initialize(this);

			foreach (Tuple<StateKey, object> tuple in listIn)
			{
				mapper.SetUnchecked(tuple.Item1, tuple.Item2);//A Forced Placing!
			}
			ValidStates.Add(mapper);
			Containers.Add(mapper);
		}
	}

	public int IdFor(StateContainer t)
	{
		return Containers.IdFor(t);
	}

	public StateContainer Get(int id)
	{
		if (id >= Containers.Count) return Containers.FromId(0);
		return Containers.FromId(id);
	}

}
