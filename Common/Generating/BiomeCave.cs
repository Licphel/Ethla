using System;
using Ethla.World.Generating;

namespace Ethla.Common.Generating;

public class BiomeCave : Biome
{

    public BiomeCave()
	{
		Rainfall = 0.5f;
		Temperature = 0.5f;
		Activeness = 0.1f;
		Continent = 0.5f;
		Rarity = 0.5f;
        Depth = -0.5f;

		BiomeStates[0] = Blocks.Gravel.MakeState();
		BiomeStates[1] = Blocks.Gravel.MakeState();
		BiomeStates[2] = Blocks.Rock.MakeState();
		BiomeStates[3] = Blocks.Rock.MakeState();
		BiomeWalls[0] = Walls.Rock;
		BiomeWalls[1] = Walls.Rock;
		BiomeWalls[2] =	Walls.Rock;
		BiomeWalls[3] = Walls.Rock;
	}

}
