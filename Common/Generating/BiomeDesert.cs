using System;
using Ethla.World.Generating;

namespace Ethla.Common.Generating;

public class BiomeDesert : Biome
{

    public BiomeDesert()
	{
		Rainfall = 0.1f;
		Temperature = 0.8f;
		Activeness = 0.2f;
		Continent = 1.0f;
		Rarity = 0.15f;

		BiomeStates[0] = Blocks.Sand.MakeState();
		BiomeStates[1] = Blocks.Sand.MakeState();
		BiomeStates[2] = Blocks.Sand.MakeState();
		BiomeStates[3] = Blocks.Sandstone.MakeState();
		BiomeWalls[0] = Walls.Sand;
		BiomeWalls[1] = Walls.Sand;
		BiomeWalls[2] =	Walls.Sand;
		BiomeWalls[3] = Walls.Sandstone;
	}

}
