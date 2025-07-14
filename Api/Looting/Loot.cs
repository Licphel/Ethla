using Ethla.World.Iteming;
using Spectrum.Maths.Random;

namespace Ethla.Api.Looting;

public class Loot
{

	public List<Roll> BonusRolls = new List<Roll>();
	public List<Roll> Rolls = new List<Roll>();

	public void Generate(List<ItemStack> stacks, Seed seed, int bonusLevel = 0)
	{
		foreach (Roll r in Rolls)
			for (int i = 0; i < r.Times; i++)
				if (seed.NextFloat() <= r.Chance)
					stacks.Add(r.Stack.Copy());

		for (int i = 0; i < bonusLevel; i++)
			foreach (Roll r in BonusRolls)
				for (int j = 0; j < r.Times; j++)
					if (seed.NextFloat() <= r.Chance)
						stacks.Add(r.Stack.Copy());
	}

	public class Roll
	{

		public float Chance;

		public ItemStack Stack;
		public int Times;

	}

}
