using Ethla.Api.Reciping;
using Ethla.Client.Iteming;
using Ethla.Common;
using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public class GuiInventory : GuiSlotLayout
{

	private Selection selection;

	public GuiInventory(SlotLayout container) : base(container, Images.GuiInventory, new Vector2(219, 211), Lore.Translate("ethla:text.inventory"))
	{
	}

	public override void DrawUnderTooltips(Graphics graphics)
	{
		foreach (Selection.Entry entry0 in selection.Entries)
		{
			GuiPlayerInvEntry entry = entry0 as GuiPlayerInvEntry;

			if (entry.IsHovering()) GuiPlayerInvEntry.Selected = entry;
			if (entry != GuiPlayerInvEntry.Selected) continue;

			float x1 = selection.Bound.Xprom + 8;
			float y1 = selection.Bound.Yprom - 6 - 8;

			graphics.DrawText(I18N.GetText("ethla:text.recipe_ingredients"), x1, y1);

			int i = 1;
			int j = 0;

			foreach (RecipeMatcher matcher in entry.Recipe.GetMatchers())
			{
				matcher.DrawSymbol(graphics, x1 + j * 18, y1 - i * 18, 16, 16);
				float x = Cursor.X;
				float y = Cursor.X;

				i++;

				if (i > 3)
				{
					j++;
					i = 1;
				}
			}
		}
	}

	public override List<Lore> CollectTooltips()
	{
		List<Lore> lst = base.CollectTooltips();

		foreach (Selection.Entry entry0 in selection.Entries)
		{
			GuiPlayerInvEntry entry = entry0 as GuiPlayerInvEntry;

			if (entry != GuiPlayerInvEntry.Selected) continue;

			float x1 = selection.Bound.Xprom + 8;
			float y1 = selection.Bound.Yprom - 6 - 8;

			int i = 1;
			int j = 0;

			foreach (RecipeMatcher matcher in entry.Recipe.GetMatchers())
			{
				float x = Cursor.X;
				float y = Cursor.Y;

				if (x >= x1 + j * 18 && x <= x1 + j * 18 + 16 && y >= y1 - i * 18 && y <= y1 - i * 18 + 16)
					matcher.TooltipSymbol(lst);

				i++;

				if (i > 3)
				{
					j++;
					i = 1;
				}
			}
		}

		return lst;
	}

	public override void InitComponents()
	{
		base.InitComponents();

		selection = new Selection();
		selection.EntryH = 22;

		foreach (Recipe recipe in RecipeManager.GetRecipes(Recipes.Handcraft))
			selection.Add(new GuiPlayerInvEntry(Layout, recipe));

		Join(selection);
	}

	public override void RelocateComponents()
	{
		base.RelocateComponents();

		selection.Bound.Resize(30, 86);
		selection.Bound.Locate(I + 6, J + 103);
		selection.Tick();
	}

	public class GuiPlayerInvEntry : Selection.Entry
	{

		public static Selection.Entry Selected;

		public SlotLayout Menu;
		public Recipe Recipe;

		public GuiPlayerInvEntry(SlotLayout menu, Recipe recipe)
		{
			Menu = menu;
			Recipe = recipe;
		}

		public override void Draw(Graphics graphics)
		{
			graphics.DrawImage(Images.RecipeEntry, X, Y, 22, 22, 0, 0, 22, 22);
			ItemStack stack = Recipe.GetOutputs()[0];
			ItemModels.GetRenderer(stack).Draw(graphics, X + 3, Y + 3, 16, 16, stack);

			if (IsHovering())
			{
				RecipeSource src = new RecipeSourcePlayer(Menu.Player);
				if (Recipe.Matches(src))
					graphics.Color4(0.3f, 1f, 0.4f);
				else
					graphics.Color4(1f, 0.3f, 0.3f);
				graphics.DrawImage(Images.RecipeEntry, X, Y, 22, 22, 0, 22, 22, 22);
				graphics.NormalizeColor();
			}
		}

		public override bool IsHovering()
		{
			return base.IsHovering() && Cursor.X < X + 22;
		}

		public override void Pressed()
		{
			RecipeSource src = new RecipeSourcePlayer(Menu.Player);
			Recipe.Assemble(src);
		}

		public override void CollectTooltips(List<Lore> list)
		{
			if (IsHovering())
				Recipe.GetOutputs()[0].GetTooltips(list);
		}

	}

}
