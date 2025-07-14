using Ethla.Api.Grouping;

namespace Ethla.Common;

public class Groups
{

	public static Group Rock = GroupManager.Get("ethla:rock");
	public static Group Dirt = GroupManager.Get("ethla:dirt");
	public static Group Carvable = GroupManager.Get("ethla:carvable");
	public static Group Log = GroupManager.Get("ethla:log");
	public static Group Sand = GroupManager.Get("ethla:sand");
	public static GroupMap Fuel = GroupManager.Get("ethla:fuel");

}
