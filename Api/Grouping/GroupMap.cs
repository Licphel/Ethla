using Spectrum.Core.Manage;

namespace Ethla.Api.Grouping;

public class GroupMap : Group
{

	public Dictionary<Id, object> Constituents = new Dictionary<Id, object>();
	public Dictionary<Id, object> References = new Dictionary<Id, object>();

	public override void DecodeReferences()
	{
		foreach (KeyValuePair<Id, object> kv in References)
		{
			Group group = GroupManager.Get(kv.Key);
			group.TryDecodeReferences();
			foreach (Id id1 in group.GetContents())
				Constituents[id1] = kv.Value;
		}
	}

	public dynamic Valueof<T>(T o) where T : IdHolder
	{
		return Constituents[o.Uid];
	}

	public dynamic Valueof(Id key)
	{
		return Constituents[key];
	}

	public override bool Contains<T>(T o)
	{
		return Constituents.ContainsKey(o.Uid);
	}

	public override IEnumerable<Id> GetContents()
	{
		return Constituents.Keys;
	}

}
