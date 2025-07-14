using Ethla.Api.Grouping;
using Ethla.World.Iteming;
using Spectrum.IO.Bin;
using Spectrum.Utils;

namespace Ethla.Api.Reciping;

public static class RecipeDecodeHelper
{

	public static ItemStack StaticItem(BinaryList list)
	{
		string key = list.Get<string>(0);

		Item type = ModRegistry.Items[key];
		if (type == null)
		{
			Logger.Warn($"Parsing recipe error! Item: {key} was not found.");
			return ItemStack.Empty;
		}

		return type.MakeStack(list.Count == 1 ? 1 : list.Get<int>(1));
	}

	public static (Group, int) GroupItem(BinaryList list)
	{
		string key = list.Get<string>(0).Substring(1);

		Group group = GroupManager.Get(key);
		if (group == null)
		{
			Logger.Warn($"Parsing recipe error! Item group: {key} was not found.");
			return (null, 0);
		}

		return (group, list.Count == 1 ? 1 : list.Get<int>(1));
	}

}
