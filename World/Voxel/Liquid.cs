using Ethla.Api;
using Ethla.Common;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Core.Manage;
using Spectrum.Maths.Random;

namespace Ethla.World.Voxel;

public class Liquid : IdHolder, DamageSource
{

	public const int MaxAmount = 128;

	public static int SimVerticalDist = 32;
	public static int SimHorizontalDist = 32;

	private static bool lastLeft;
	private static bool spreadL;
	private static int px, py;

	public static Liquid Empty => ModRegistry.Liquids.DefaultValue;

	public virtual string GetDamageSourceInfo(Damage damage)
	{
		return GetLocalizationName();
	}

	public static void TickChunkLiquid(Chunk chunk, bool careRange = true)
	{
		px = Main.Player.Pos.BlockX;
		py = Main.Player.Pos.BlockY;

		Chunk chunk0 = chunk.Level.GetChunk(chunk.Coord - 1);
		Chunk chunk1 = chunk.Level.GetChunk(chunk.Coord + 1);

		spreadL = Seed.Global.NextFloat() < 0.5f;

		if (!lastLeft)
			for (int x = 0; x < 16; x++)
			{
				if (careRange && Math.Abs(x + chunk.Coord * 16 - px) >= SimHorizontalDist)
					continue;
				spreadYLiquid(x);
			}
		else
			for (int x = 15; x >= 0; x--)
			{
				if (careRange && Math.Abs(x + chunk.Coord * 16 - px) >= SimHorizontalDist)
					continue;
				spreadYLiquid(x);
			}

		lastLeft = !lastLeft;

		return;

		void spreadYLiquid(int x)
		{
			if (careRange)
			{
				int my = Math.Min(Chunk.MaxY, py + SimVerticalDist);
				for (int y = Math.Max(0, py - SimVerticalDist); y <= my; y++)
					spreadLiquid(chunk, chunk0, chunk1, x, y, chunk.LiquidMap);
			}
			else
			{
				for (int y = 0; y <= Chunk.MaxY; y++)
					spreadLiquid(chunk, chunk0, chunk1, x, y, chunk.LiquidMap);
			}
		}
	}

	private static void spreadLiquid(Chunk chunk, Chunk chunk0, Chunk chunk1, int x, int y, LiquidMap arr)
	{
		int realx = x + chunk.Coord * 16;
		LiquidStack stack = arr.Get(x, y);
		Liquid type = stack.Liquid;
		int a = stack.Amount;

		if (a == 0 || stack.Liquid == Empty) return;

		{
			BlockState bd = chunk.GetBlock(x, y - 1);
			LiquidStack stackd = arr.Get(x, y - 1);
			int ad = stackd.Amount;
			Liquid ld = stackd.Liquid;

			if (bd.GetShape() != BlockShape.Solid)
			{
				if (ld != Empty && ld != type)
				{
					type.OnTouch(stack, stackd, chunk.Level, realx, y, realx, y - 1);
				}
				else if (ad < MaxAmount)
				{
					int ext = Math.Min(MaxAmount - ad, a);
					a -= ext;
					arr.Set(x, y, new LiquidStack(type, a));
					arr.Set(x, y - 1, new LiquidStack(type, ext + ad));

					if (ext > a / 2)
						return;
				}
			}
		}
		/*
		else
		{
		    BlockState bu = chunk.GetBlock(x, y + 1);
		    LiquidStack stacku = arr.Get(x, y + 1);
		    int au = stacku.Amount;
		    Liquid lu = stacku.Liquid;

		    if(bu.GetShape() != BlockShape.Solid)
		    {
		        if(lu != Empty && lu != type)
		        {
		            type.OnTouch(stack, stacku, chunk.Level, realx, y, realx, y + 1);
		        }
		        else if(au < MaxAmount)
		        {
		            int ext = Math.Min(MaxAmount - au, a);
		            a -= ext;
		            arr.Set(x, y, new LiquidStack(type, a));
		            arr.Set(x, y + 1, new LiquidStack(type, ext + au));

		            if(ext > a / 2)
		                return;
		        }
		    }
		}
		*/

		BlockState bl, br;
		LiquidMap arr0, arr1;
		int ar, al;
		LiquidStack lsr, lsl;
		Liquid lr, ll;

		if (x == 0)
		{
			if (chunk0 == null)
				return;
			bl = chunk0.GetBlock(x - 1, y);
			br = chunk.GetBlock(x + 1, y);
			arr0 = chunk0.LiquidMap;
			arr1 = chunk.LiquidMap;
			lsr = arr.Get(x + 1, y);
			lsl = arr0.Get(x - 1, y);
			ar = lsr.Amount;
			al = lsl.Amount;
			lr = lsr.Liquid;
			ll = lsl.Liquid;
		}
		else if (x == 15)
		{
			if (chunk1 == null)
				return;
			bl = chunk.GetBlock(x - 1, y);
			br = chunk1.GetBlock(x + 1, y);
			arr0 = chunk.LiquidMap;
			arr1 = chunk1.LiquidMap;
			lsr = arr1.Get(x + 1, y);
			lsl = arr.Get(x - 1, y);
			ar = lsr.Amount;
			al = lsl.Amount;
			lr = lsr.Liquid;
			ll = lsl.Liquid;
		}
		else
		{
			bl = chunk.GetBlock(x - 1, y);
			br = chunk.GetBlock(x + 1, y);
			arr0 = chunk.LiquidMap;
			arr1 = chunk.LiquidMap;
			lsr = arr.Get(x + 1, y);
			lsl = arr.Get(x - 1, y);
			ar = lsr.Amount;
			al = lsl.Amount;
			lr = lsr.Liquid;
			ll = lsl.Liquid;
		}

		if (spreadL && bl.GetShape() != BlockShape.Solid)
		{
			if (ll != Empty && ll != type)
			{
				type.OnTouch(stack, lsl, chunk.Level, realx, y, realx - 1, y);
			}
			else
			{
				int ext = Math.Min(MaxAmount - al, Math.Min((int)Math.Ceiling((a - al) / 2f), a));
				a -= ext;
				arr.Set(x, y, new LiquidStack(type, a));
				arr0.Set(x - 1, y, new LiquidStack(type, ext + al));
			}
		}
		else if (br.GetShape() != BlockShape.Solid)
		{
			if (lr != Empty && lr != type)
			{
				type.OnTouch(stack, lsr, chunk.Level, realx, y, realx + 1, y);
			}
			else
			{
				int ext = Math.Min(MaxAmount - ar, Math.Min((int)Math.Ceiling((a - ar) / 2f), a));
				a -= ext;
				arr.Set(x, y, new LiquidStack(type, a));
				arr1.Set(x + 1, y, new LiquidStack(type, ext + ar));
			}
		}
	}

	public virtual float GetDensity()
	{
		return 20.62f;
	}

	public virtual float GetFriction()
	{
		return 18.5f;
	}

	public virtual float CastLight(byte pipe, int x, int y, int am)
	{
		return 0;
	}

	public virtual float FilterLight(byte pipe, float v, int x, int y, int am)
	{
		return v * 0.98f - am / MaxAmount * 0.01f;
	}

	public virtual void OnEntityCollided(Entity entity, float intersection)
	{
		if (intersection >= entity.Bound.Area * 0.99f && entity is Creature creature)
		{
			creature.Hit(new Damage(this, DamageTypes.Biotic, 0.5f));
		}
	}

	public virtual void OnTouch(LiquidStack stack, LiquidStack other, Level level, int x, int y, int x1, int y1)
	{
	}

	public virtual string GetLocalizationName()
	{
		return I18N.GetText($"{Uid.Space}:liquid.{Uid.Key}");
	}

}
