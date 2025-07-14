using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.IO.Bin;

namespace Ethla.Api.Reciping;

public interface Recipe
{

	Id Path { get; }
	RecipeCategory Category { get; }

	bool Matches(RecipeSource source);

	//The Assemble method should do:
	//1. Check if the recipe is matched, generally call #Matches.
	//2. Check if the destination is accessible.
	//3. If all fine, set the output to the source.
	bool Assemble(RecipeSource source);

	List<RecipeMatcher> GetMatchers();

	List<ItemStack> GetOutputs();

	void Read(BinaryCompound compound, Id id);

}
