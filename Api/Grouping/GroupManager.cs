using Spectrum.Core.Manage;

namespace Ethla.Api.Grouping;

public class GroupManager
{

	public static Dictionary<Id, Group> Dictionary = new Dictionary<Id, Group>();

	public static dynamic Get(Id key)
	{
		Group g = Dictionary.GetValueOrDefault(key, null);
		g.TryDecodeReferences();
		return g;
	}

}
