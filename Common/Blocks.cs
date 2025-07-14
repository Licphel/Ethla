using Ethla.Api;
using Ethla.Common.Voxel;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Blocks
{

	public static readonly IdMap<Block> Registry = ModRegistry.Blocks;

	public static Block Empty = Registry.RegisterDefaultValue("ethla:empty", new Block());
	public static Block Border = Registry.Register("ethla:border", new BlockBorder()).DeriveItem();
	public static Block Flower = Registry.Register("ethla:flower", new BlockPlant());
	public static Block Grass = Registry.Register("ethla:grass", new BlockPlant());
	public static Block Bush = Registry.Register("ethla:bush", new BlockPlant());
	public static Block Vine = Registry.Register("ethla:vine", new BlockVine());
	public static Block BirchSeedling = Registry.Register("ethla:birch_seedling", new BlockSeedling(() => Features.BirchTree)).DeriveItem();
	public static Block BirchLog = Registry.Register("ethla:birch_log", new Block()).DeriveItem();
	public static Block BirchLeaves = Registry.Register("ethla:birch_leaves", new Block()).DeriveItem();
	public static Block MapleSeedling = Registry.Register("ethla:maple_seedling", new BlockSeedling(() => Features.MapleTree)).DeriveItem();
	public static Block MapleLog = Registry.Register("ethla:maple_log", new Block()).DeriveItem();
	public static Block MapleLeaves = Registry.Register("ethla:maple_leaves", new Block()).DeriveItem();
	public static Block Cactus = Registry.Register("ethla:cactus", new BlockCactus()).DeriveItem();
	public static Block Dirt = Registry.Register("ethla:dirt", new BlockDirt()).DeriveItem();
	public static Block Clay = Registry.Register("ethla:clay", new BlockDirt()).DeriveItem();
	public static Block Turf = Registry.Register("ethla:turf", new BlockDirt()).DeriveItem();
	public static Block Gravel = Registry.Register("ethla:gravel", new Block()).DeriveItem();
	public static Block Rock = Registry.Register("ethla:rock", new Block()).DeriveItem();
	public static Block Sand = Registry.Register("ethla:sand", new Block()).DeriveItem();
	public static Block Sandstone = Registry.Register("ethla:sandstone", new Block()).DeriveItem();
	public static Block CoalOre = Registry.Register("ethla:coal_ore", new Block()).DeriveItem();
	public static Block IronOre = Registry.Register("ethla:iron_ore", new Block()).DeriveItem();
	public static Block GoldOre = Registry.Register("ethla:gold_ore", new Block()).DeriveItem();
	public static Block Obsidian = Registry.Register("ethla:obsidian", new Block()).DeriveItem();
	public static Block Lamp = Registry.Register("ethla:lamp", new BlockWallCeilAttach()).DeriveItem();
	public static Block Chest = Registry.Register("ethla:chest", new BlockChest()).DeriveItem();
	public static Block Furnace = Registry.Register("ethla:furnace", new BlockFurnace()).DeriveItem();
	public static Block Torch = Registry.Register("ethla:torch", new BlockWallLandAttach()).DeriveItem();

}
