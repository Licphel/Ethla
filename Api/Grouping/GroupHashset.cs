using Spectrum.Core.Manage;

namespace Ethla.Api.Grouping;

public class GroupHashset : Group
{

	public HashSet<Id> Constituents = new HashSet<Id>();
	public List<Id> References = new List<Id>();

	public override void DecodeReferences()
	{
		foreach (Id id in References)
		{
			Group group = GroupManager.Get(id);
			group.TryDecodeReferences();
			foreach (Id id1 in group.GetContents())
				Constituents.Add(id1);
		}
	}

	public override bool Contains<T>(T o)
	{
		return Constituents.Contains(o.Uid);
	}

	public override IEnumerable<Id> GetContents()
	{
		return Constituents;
	}

}
