using Spectrum.Core.Manage;

namespace Ethla.Api.Grouping;

public abstract class Group : IdHolder
{

	public bool Decoded;

	public void TryDecodeReferences()
	{
		if (Decoded)
			return;
		DecodeReferences();
		Decoded = true;
	}

	public virtual void DecodeReferences() { }

	public abstract bool Contains<T>(T o) where T : IdHolder;

	public virtual IEnumerable<Id> GetContents()
	{
		return Array.Empty<Id>();
	}

	public static Group Combine(string id, params Group[] groups)
	{
		return Combine(new Id(id), groups);
	}

	public static Group Combine(Id id, params Group[] groups)
	{
		GroupHashset groupc = new GroupHashset();
		foreach (Group group in groups)
			foreach (Id o in group.GetContents())
				groupc.Constituents.Add(o);
		//Try add back.
		GroupManager.Dictionary[id] = groupc;
		return groupc;
	}

}
