using Ethla.Api.Grouping;
using Ethla.Api.Looting;
using Ethla.Api.Reciping;
using Ethla.Client.Voxel;
using Ethla.Util;
using Ethla.World.Iteming;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Labyrinth;
using Labyrinth.Values;
using Spectrum.Core.Manage;
using Spectrum.IO;
using Spectrum.IO.Bin;
using static Ethla.Client.Voxel.BlockModels;
using static Ethla.Client.Voxel.WallModels;

namespace Ethla.Api.Scripting;

public static class ScriptDecoder
{

	public static void SetBlockProperty(Id id, BinaryCompound compound)
	{
		BlockProperty property = new BlockProperty();
		property.Hardness = compound.GetOrDefault("hardness", 0f);
		property.Material = ModRegistry.BlockMaterials[compound.GetOrDefault<string>("material", "ethla:unknown")];
		property.Shape = compound.GetOrDefault<string>("shape", "solid") switch
		{
			"vaccum" => BlockShape.Vacuum,
			"partial" => BlockShape.Parital,
			"hollow" => BlockShape.Hollow,
			"solid" => BlockShape.Solid,
			_ => throw new Exception("Unknown Block Shape.")
		};
		property.Friction = compound.GetOrDefault("friction", 1f);
		property.Floating = compound.GetOrDefault("floating", 0f);
		property.Preventing = compound.GetOrDefault("preventing", 0f);

		property.WallAttach = compound.GetOrDefault("wall_attach", false);
		property.CanBeReplace = compound.GetOrDefault("can_be_replaced", false);

		if (compound.Try<BinaryList>("silhouette", out BinaryList phyout))
			property.Silhouette = GetClip(phyout);

		if (compound.Try<BinaryList>("light", out BinaryList llist))
		{
			property.Light[0] = llist.Get<float>(0);
			property.Light[1] = llist.Get<float>(1);
			property.Light[2] = llist.Get<float>(2);
		}

		if (compound.Try<BinaryList>("loot", out BinaryList lootraw))
			AddLoot(id, lootraw, "block");

		property.Rarity = compound.GetOrDefault("rarity", 0);

		Block block = ModRegistry.Blocks.GetStrictly(id);
		block.Property = property;

		static VoxelClip GetClip(BinaryList list)
		{
			VoxelOutline outline = new VoxelOutline();
			foreach (BinaryList l1 in list)
				outline.Add(new VoxelBox(l1.Get<float>(0), l1.Get<float>(1), l1.Get<float>(2), l1.Get<float>(3)));
			return outline;
		}
	}

	public static void SetWallProperty(Id id, BinaryCompound compound)
	{
		WallProperty property = new WallProperty();
		property.Hardness = compound.GetOrDefault("hardness", 0f);
		property.Material = ModRegistry.BlockMaterials[compound.GetOrDefault<string>("material", "ethla:unknown")];
		property.Shape = compound.GetOrDefault<string>("shape", "solid") switch
		{
			"vaccum" => BlockShape.Vacuum,
			"partial" => BlockShape.Parital,
			"hollow" => BlockShape.Hollow,
			"solid" => BlockShape.Solid,
			_ => throw new Exception("Unknown Block Shape.")
		};

		property.CanBeReplace = compound.GetOrDefault("can_be_replaced", false);

		if (compound.Try<BinaryList>("light", out BinaryList llist))
		{
			property.Light[0] = llist.Get<float>(0);
			property.Light[1] = llist.Get<float>(1);
			property.Light[2] = llist.Get<float>(2);
		}

		if (compound.Try<BinaryList>("loot", out BinaryList lootraw))
			AddLoot(id, lootraw, "block");

		property.Rarity = compound.GetOrDefault("rarity", 0);

		Wall wall = ModRegistry.Walls.GetStrictly(id);
		wall.Property = property;
	}

	public static void SetItemProperty(Id id, BinaryCompound compound)
	{
		ItemProperty property = new ItemProperty();

		if (compound.Try<BinaryList>("light", out BinaryList llist))
		{
			property.Light[0] = llist.Get<float>(0);
			property.Light[1] = llist.Get<float>(1);
			property.Light[2] = llist.Get<float>(2);
		}

		property.MaxStackSize = compound.GetOrDefault("stack_size", 30);
		property.RemoteUsage = compound.GetOrDefault("remote_usage", false);
		property.ToolType = ItemToolType.ToolTypeMap[compound.GetOrDefault<string>("tool_type", "none")];
		if (compound.Try<BinaryCompound>("tier", out BinaryCompound tierraw))
			property.Tier = GetTier(tierraw);

		property.Rarity = compound.GetOrDefault("rarity", 0);

		Item item = ModRegistry.Items.GetStrictly(id);
		item.Property = property;

		static Tier GetTier(BinaryCompound tierComp)
		{
			BinaryList list = tierComp.GetListSafely("multiplier");
			return new Tier(tierComp.Get<int>("level"), new Multiplier(list.Get<float>(0), list.Get<float>(1)));
		}
	}

	public static void SetPrefabProperty(Id id, BinaryCompound compound)
	{
		EntityProperty property = new EntityProperty();
		property.Lowp = compound.GetOrDefault("lowp", false);
		property.Mass = compound.GetOrDefault("mass", 0f);
		property.IsCollidable = compound.GetOrDefault("collision", true);
		property.IsSlidable = compound.GetOrDefault("slide", true);
		property.ReboundFactor = compound.GetOrDefault("rebound", 0f);

		if (compound.Try<BinaryList>("silhouette", out BinaryList phyout))
		{
			property.BoundX = phyout.Get<float>(0) / 16;
			property.BoundY = phyout.Get<float>(1) / 16;
		}

		if (compound.Try<BinaryList>("visual", out BinaryList visout))
		{
			property.VisualX = visout.Get<float>(0) / 16;
			property.VisualY = visout.Get<float>(1) / 16;
		}

		if (compound.Try<BinaryList>("light", out BinaryList llist))
		{
			property.Light[0] = llist.Get<float>(0);
			property.Light[1] = llist.Get<float>(1);
			property.Light[2] = llist.Get<float>(2);
		}

		if (compound.Try<BinaryList>("loot", out BinaryList lootraw))
			AddLoot(id, lootraw, "entity");

		Prefab prefab = ModRegistry.Prefabs[id];
		if (prefab == null)
			throw new Exception($"Unknown Prefab: '{id}'.");

		prefab.Property = property;
	}

	public static void AddBlockModels(BinaryList list)
	{
		foreach (BinaryCompound c in list)
		{
			Id modelpath = c.Get<string>("model");
			ExecutionResult result = ExecutionResult.From(new TextAccess(modelpath));
			BinaryCompound model = result.Result<BinaryCompound>();
			string renderer = model.Get<string>("renderer");
			string mask = model.Get<string>("mask");
			BinaryList statemap = model.GetListSafely("statemap");

			foreach (string blockname in c.GetListSafely("values"))
			{
				Id blockid = blockname;
				foreach (BinaryList idpair in statemap)
				{
					BlockRenderData data = new BlockRenderData();
					data.Renderer = BlockRenderer.Renderers[renderer];
					data.Mask = Loads.Get(mask);
					data.Icon = Loads.Get(idpair.Get<string>(1).Replace("${self_key}", blockid.Key).Replace("${self_space}", blockid.Space));
					Block block = ModRegistry.Blocks.GetStrictly(blockid);
					BlockModels.DataMap[IntPair.Create(block.Uid, idpair.Get<int>(0))] = data;
				}
			}
		}
	}

	public static void AddWallModels(BinaryList list)
	{
		foreach (BinaryCompound c in list)
		{
			Id modelpath = c.Get<string>("model");
			ExecutionResult result = ExecutionResult.From(new TextAccess(modelpath));
			BinaryCompound model = result.Result<BinaryCompound>();
			string renderer = model.Get<string>("renderer");
			string mask = model.Get<string>("mask");
			string image = model.Get<string>("image");

			foreach (string wallname in c.GetListSafely("values"))
			{
				Id wallid = wallname;
				WallRenderData data = new WallRenderData();
				data.Renderer = WallRenderer.Renderers[renderer];
				data.Mask = Loads.Get(mask);
				data.Icon = Loads.Get(image.Replace("${self_key}", wallid.Key).Replace("${self_space}", wallid.Space));
				Wall wall = ModRegistry.Walls.GetStrictly(wallid);
				WallModels.DataMap[wall.Uid] = data;
			}
		}
	}

	public static void AddGroup(Id id, object value)
	{
		if (value is BinaryList list)
		{
			GroupHashset gh = new GroupHashset();
			foreach (string s in list)
			{
				if (s.StartsWith('#'))
					gh.References.Add(s.Substring(1));
				else
					gh.Constituents.Add(new Id(s));
			}
			gh.Uid = id;
			GroupManager.Dictionary[id] = gh;
		}
		else if (value is BinaryCompound compound)
		{
			GroupMap gm = new GroupMap();
			foreach (KeyValuePair<string, object> kv in compound)
			{
				string s = kv.Key;
				object o = kv.Value;
				if (s.StartsWith('#'))
					gm.References[s.Substring(1)] = o;
				else
					gm.Constituents[s] = o;
			}
			gm.Uid = id;
			GroupManager.Dictionary[id] = gm;
		}
	}

	public static void AddLoot(Id id, BinaryList list, string type)
	{
		HashSet<object> set = new HashSet<object>();

		Loot loot = new Loot();

		foreach (BinaryCompound compound in list)
		{
			Item item = ModRegistry.Items[compound.Get<string>("id")];
			ItemStack stack = item.MakeStack(compound.GetOrDefault("count", 1));
			bool bonus = compound.GetOrDefault("bonus", false);
			float chance = compound.GetOrDefault<float>("chance", 1);
			int times = compound.GetOrDefault("times", 1);
			Loot.Roll roll = new Loot.Roll { Stack = stack, Chance = chance, Times = times };
			if (bonus)
				loot.BonusRolls.Add(roll);
			else
				loot.Rolls.Add(roll);
		}

		if (type == "block")
			LootManager.BlockDict[id] = loot;
		if (type == "entity")
			LootManager.EntityDict[id] = loot;
	}

	public static void AddRecipe(Id id, BinaryCompound compound)
	{
		RecipeCategory category = ModRegistry.RecipeCategories[compound.Get<string>("category")];

		Recipe recipe = category.Sup();
		recipe.Read(compound, id);

		RecipeManager.RecipeMap[id] = recipe;

		if (!RecipeManager.TypeMap.ContainsKey(category))
			RecipeManager.TypeMap[category] = new Dictionary<Id, Recipe>();
		Dictionary<Id, Recipe> lst = RecipeManager.TypeMap[category];
		lst[id] = recipe;
	}

}
