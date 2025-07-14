using Ethla.World.Generating;

namespace Ethla.Common.Generating;

public class BiomeForest : Biome
{

	public BiomeForest()
	{
		Rainfall = 0.8f;
		Temperature = 0.5f;
		Activeness = 0.3f;
		Continent = 1.0f;
		Rarity = 0.1f;

		BiomeStates[0] = Blocks.Dirt.MakeState(1);
		BiomeStates[1] = Blocks.Dirt.MakeState();
		BiomeStates[2] = Blocks.Gravel.MakeState();
		BiomeStates[3] = Blocks.Rock.MakeState();
		BiomeWalls[0] = Walls.Dirt;
		BiomeWalls[1] = Walls.Dirt;
		BiomeWalls[2] = Walls.Rock;
		BiomeWalls[3] = Walls.Rock;
	}

}
