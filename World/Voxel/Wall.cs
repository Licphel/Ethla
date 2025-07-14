using System;
using Ethla.Api;
using Ethla.Api.Grouping;
using Ethla.Api.Looting;
using Ethla.Client.Audio;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Spectrum.Core.Manage;
using Spectrum.Maths.Random;

namespace Ethla.World.Voxel;

public class Wall : Group
{

    public static Wall Empty => ModRegistry.Walls.DefaultValue;

    public Item Item;
    public WallProperty Property = new WallProperty();

    public Wall DeriveItem(Item item = null)
	{
		Item ip = item ?? new ItemWall(this);
		Item = ModRegistry.Items.Register($"{Uid.Space}:{Uid.Key}_wall", ip);
		return this;
	}

    public override bool Contains<T>(T o)
    {
        return o is Wall wall && wall == this;
    }

    public virtual string GetLocalizationName()
    {
        return I18N.GetText($"{Uid.Space}:wall.{Uid.Key}");
    }

    public virtual Sound GetSound(string type)
    {
        BlockMaterial material = GetMaterial();
        return material.GetNormalSound(type);
    }

    public virtual BlockMaterial GetMaterial()
    {
        return Property.Material;
    }

    public virtual BlockShape GetShape()
    {
        return Property.Shape;
    }

    public virtual float GetHardness()
    {
        return Property.Hardness;
    }

    public virtual bool CanBeReplace()
    {
        return this == Empty || Property.CanBeReplace;
    }

    public virtual float CastLight(byte pipe, int x, int y)
    {
        return Property.Light[pipe];
    }

    public virtual float FilterLight(byte pipe, float v, int x, int y)
    {
        return GetShape().FilterLight(pipe, v, x, y);
    }

    public virtual List<ItemStack> GetDrop(Level level, BlockPos pos)
    {
        List<ItemStack> list = new List<ItemStack>();

        Loot loot = LootManager.GetForWall(Uid);

        if (loot == null)
        {
            if (this != Empty && Item != null)
                list.Add(Item.MakeStack());
        }
        else
        {
            loot.Generate(list, Seed.Global);
        }

        return list;
    }
    
    //Events
    
    public virtual void OnDestroyed(DamageSource src, BlockPos pos)
    {
    }

}
