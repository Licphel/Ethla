using Ethla.Api.Grouping;
using Ethla.Client.Iteming;
using Ethla.World.Iteming;
using Spectrum.Core;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Text;
using Spectrum.IO.Bin;

namespace Ethla.Api.Reciping;

public interface RecipeMatcher
{

	int Count { get; }

	public static RecipeMatcher Read(BinaryList list)
	{
		if (list.Get<string>(0).StartsWith('#'))
		{
			(Group, int) tuple = RecipeDecodeHelper.GroupItem(list);
			return new GroupedItem(tuple.Item1, tuple.Item2);
		}

		return new StaticItem(RecipeDecodeHelper.StaticItem(list));
	}

	bool Test(RecipeSource source, Func<int, bool> filter);

	void Consume(RecipeSource source);

	void DrawSymbol(Graphics graphics, float x, float y, float w, float h);

	void TooltipSymbol(List<Lore> tooltips);

	class StaticItem : RecipeMatcher
	{

		private readonly ItemStack stack;

		public StaticItem(ItemStack stack)
		{
			this.stack = stack;
		}

		public StaticItem(Item type, int count)
		{
			stack = type.MakeStack(count);
		}

		public StaticItem(Item type)
		{
			stack = type.MakeStack();
		}

		public int Count => stack.Count;

		public bool Test(RecipeSource source, Func<int, bool> filter)
		{
			int i = 0;
			for (int k = 0; k < source.ItemContainer.Count; k++)
			{
				if (!filter(k))
					continue;
				ItemStack stk = source.ItemContainer[k];
				if (stk.IsCompatible(stack) && !stk.IsEmpty)
					i += stk.Count;
			}

			return i >= Count;
		}

		public void Consume(RecipeSource source)
		{
			source.ItemContainer.Extract(stack);
		}

		public void DrawSymbol(Graphics graphics, float x, float y, float w, float h)
		{
			ItemModels.GetRenderer(stack).Draw(graphics, x, y, w, h, stack, forcecount: true);
		}

		public void TooltipSymbol(List<Lore> tooltips)
		{
			stack.GetTooltips(tooltips);
		}

	}

	class GroupedItem : RecipeMatcher
	{

		private readonly Id[] array;

		private readonly Group group;
		private readonly TimeSchedule schedule;
		private int counter;

		private ItemStack itemSymbol;

		public GroupedItem(Group group, int count)
		{
			this.group = group;
			Count = count;

			array = group.GetContents().ToArray();
			schedule = new TimeSchedule();
		}

		public int Count { get; }

		public bool Test(RecipeSource source, Func<int, bool> filter)
		{
			int i = 0;
			for (int k = 0; k < source.ItemContainer.Count; k++)
			{
				if (!filter(k))
					continue;
				ItemStack stk = source.ItemContainer[k];
				if (stk.Is(group))
					i += stk.Count;
			}

			return i >= Count;
		}

		public void Consume(RecipeSource source)
		{
			source.ItemContainer.Extract(Count, s => s.Is(group));
		}

		public void DrawSymbol(Graphics graphics, float x, float y, float w, float h)
		{
			if (schedule.PeriodicTaskChecked(1f) || itemSymbol == null)
			{
				itemSymbol = ModRegistry.Items[array[counter]].MakeStack(Count);

				counter++;
				if (counter >= array.Length)
					counter = 0;
			}

			ItemModels.GetRenderer(itemSymbol).Draw(graphics, x, y, w, h, itemSymbol, forcecount: true);
		}

		public void TooltipSymbol(List<Lore> tooltips)
		{
			Lore l0 = Lore.Iconic("ethla:image/gui/tooltip/recipe_group.png", 8, 8);
			l0.Combine(Lore.Literal(group.Uid).Paint(0.45f, 0.65f, 0.85f, 0.65f));
			tooltips.Add(l0);

			tooltips.Add(Lore.Literal(itemSymbol.GetLocalizationName()));
		}

	}

}
