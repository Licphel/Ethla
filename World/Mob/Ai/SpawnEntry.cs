using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.World.Mob.Ai;

public class SpawnEntry : IdHolder
{

	public const int Anywhere = 0;
	public const int Space = 1;
	public const int Surface = 2;
	public const int Cave = 3;

	public const int Casaul = 0;
	public const int StandFirm = 1;
	public const int InAir = 2;
	public const int InWater = 3;
	public const int InLava = 4;
	public int Cap = 1;

	public int HeightSpawnCondition = Anywhere;

	public Prefab Prefab;

	public int SpaceSpawnCondition = Casaul;

	public SpawnEntry()
	{
	}

	public SpawnEntry(Prefab prefab, int cap, int hSpawn, int sSpawn)
	{
		Prefab = prefab;
		Cap = cap;
		HeightSpawnCondition = hSpawn;
		SpaceSpawnCondition = sSpawn;
	}

	public virtual bool IsValid(Level level, Pos pos)
	{
		BlockState here = level.GetBlock(pos);
		BlockState step = level.GetBlock(Direction.Down.Step(pos));

		bool c1 = false;

		switch (HeightSpawnCondition)
		{
			case Anywhere:
				c1 = true;
				break;
			case Space:
				c1 = pos.Y >= Chunk.YOfSpace;
				break;
			case Surface:
				c1 = pos.Y > Chunk.YOfSea && pos.Y < Chunk.YOfSpace;
				break;
			case Cave:
				c1 = pos.Y <= Chunk.YOfSea;
				break;
		}

		bool c2 = false;

		switch (HeightSpawnCondition)
		{
			case Casaul:
				c2 = true;
				break;
			case StandFirm:
				c2 = step.GetShape() == BlockShape.Solid && !here.GetShape().IsFull;
				break;
			case InAir:
				c2 = !step.GetShape().IsFull && !here.GetShape().IsFull;
				break;
		}

		return c1 && c2;
	}

}
