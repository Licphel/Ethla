using Spectrum.Core.Manage;

namespace Ethla.Api.Reciping;

public class RecipeManager
{

	public static readonly Dictionary<Id, Recipe> RecipeMap = new Dictionary<Id, Recipe>();
	public static readonly Dictionary<RecipeCategory, Dictionary<Id, Recipe>> TypeMap = new Dictionary<RecipeCategory, Dictionary<Id, Recipe>>();

	public static Recipe GetRecipe(Id path)
	{
		return RecipeMap[path];
	}

	public static IEnumerable<Recipe> GetRecipes(RecipeCategory type)
	{
		Dictionary<Id, Recipe> recipes = TypeMap.GetValueOrDefault(type, null);
		return recipes == null ? new List<Recipe>() : recipes.Values;
	}

}
