using Spectrum.Core.Manage;

namespace Ethla.Api.Reciping;

public class RecipeCategory : IdHolder
{

	public Func<Recipe> Sup;

	public RecipeCategory(Func<Recipe> sup)
	{
		Sup = sup;
	}

}
