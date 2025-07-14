using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.IO.Bin;

namespace Ethla.World;

public class ChunkUnit
{

	public Chunk Chunk;
	public List<Entity> Entities = new List<Entity>();
	public InheritedMap EntitySortMap = new InheritedMap();

	public Level Level;
	public UnitPos Pos;

	public ChunkUnit(Chunk chunk, UnitPos pos)
	{
		Level = chunk.Level;
		Chunk = chunk;
		Pos = pos;
	}

	public void Tick()
	{
		for (int i = Entities.Count - 1; i >= 0; i--)
		{
			if (i >= Entities.Count)
				break;

			Entity e = Entities[i];

			if (e == null)
				continue;

			if (e.IsDead)
			{
				e.DeathTimer += Time.Delta / e.DeathProcessLength;
				if (e.ShouldRemove)
				{
					RemoveEntity(e, true);
				}
			}

			if (Level.Ticks == e.LastTick) continue; //Tick wrongly invoked. When transferring this happens.
			e.Tick();
			e.LastTick = Level.Ticks;

			if (e.ChunkUnit == null) e.ChunkUnit = this;

			Pos oldpos = e.ChunkUnit.Pos;

			if (oldpos.UnitX != e.Pos.UnitX || oldpos.UnitY != e.Pos.UnitY)
			{
				ChunkUnit nunit = Level.GetUnit(e.Pos);

				if (nunit != null)
				{
					MoveEntityReference(e, nunit);
					e.ChunkUnit = nunit;
				}
			}
		}
	}

	public void RemoveEntity(Entity e, bool inLevelDim)
	{
		Entities.Remove(e);
		Spawner.DecCount(e);
		EntitySortMap.Remove(e);

		if (inLevelDim) Level.RemoveEntity(e);

		Chunk.Dirty = true;
	}

	public void AddEntity(Entity e)
	{
		Entities.Add(e);
		Spawner.RefCount(e);
		EntitySortMap.Add(e);
		Chunk.Dirty = true;
	}

	public void MoveEntityReference(Entity e, ChunkUnit newUnit)
	{
		RemoveEntity(e, false);
		newUnit.AddEntity(e);
		Chunk.Dirty = true;
	}

	//Codec

	public void Write(BinaryCompound compound, bool removal = false)
	{
		BinaryList list1 = BinaryList.New();

		for (int i = 0; i < Entities.Count; i++)
		{
			Entity e = Entities[i];
			if (removal)
			{
				Level.EntitiesById.Remove(e.UniqueId);
				Spawner.DecCount(e);
			}
			if (e == null || e is EntityPlayer)
				continue;
			BinaryCompound compound1 = BinaryCompound.New();
			e.Write(compound1);
			list1.Insert(compound1);
		}

		compound.Set("entities", list1);
	}

	public void Read(BinaryCompound compound)
	{
		BinaryList list1 = compound.GetListSafely("entities");

		foreach (BinaryCompound compound1 in list1)
		{
			Entity e = Entity.ReadFromType(compound1);
			Level.SpawnEntity(e);
		}
	}

}
