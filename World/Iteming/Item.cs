using Ethla.Api;
using Ethla.Api.Grouping;
using Ethla.Util;
using Ethla.World.Physics;
using Spectrum.Core.Manage;
using Spectrum.Graphic.Text;

namespace Ethla.World.Iteming;

public class Item : Group
{

	public ItemProperty Property = new ItemProperty();

	//INJECT
	public static Item Empty => ModRegistry.Items.DefaultValue;

	public override bool Contains<T>(T o)
	{
		return o is Item item && item == this;
	}

	public virtual ItemStack MakeStack(int count = 1)
	{
		return new ItemStack(this, count);
	}

	//END

	//Instance Relative

	public virtual float CastLight(ItemStack stack, byte pipe)
	{
		return Property.Light[pipe];
	}

	public virtual string GetLocalizationName(ItemStack stack)
	{
		return I18N.GetText($"{Uid.Space}:item.{Uid.Key}");
	}

	public virtual string GetPediaDesc(ItemStack stack, out bool hasPedia)
	{
		string key = $"{Uid.Space}:pedia.item.{Uid.Key}";
		string val = I18N.GetText(key);
		hasPedia = val != key;
		return val;
	}

	public virtual int GetStackSize(ItemStack stack)
	{
		return Property.MaxStackSize;
	}

	public virtual Tier GetTier(ItemStack stack)
	{
		return Property.Tier;
	}

	public virtual ItemToolType GetToolType(ItemStack stack)
	{
		return Property.ToolType;
	}

	public virtual float GetToolEfficiency(ItemStack stack)
	{
		return 1 * GetTier(stack).Multiplier;
	}

	public virtual void GetTooltips(ItemStack stack, List<Lore> tooltips)
	{
		string name = GetLocalizationName(stack);

		Lore l0, l1;

		tooltips.Add(Lore.Literal(name));
		tooltips.Add(Lore.Iconic("ethla:image/gui/tooltip_split.png", 32, 2, true));
		string pedia = GetPediaDesc(stack, out bool hasPedia);
		if (hasPedia)
			tooltips.Add(Lore.Literal(pedia).Paint(0.5f, 0.55f, 0.65f));

		Tier tier = GetTier(stack);

		if (tier != Tier.None)
		{
			int l = GetTier(stack).Level;
			l0 = Lore.Iconic("ethla:image/gui/tooltip/item_tier.png", 8, 8);
			l1 = Lore.Literal($"{I18N.GetText("ethla:text.tier")} {NumberForm.GetRoman(l + 1)}");
			l0.Combine(l1);
			tooltips.Add(l0);

			l0 = Lore.Iconic("ethla:image/gui/tooltip/item_multiplier.png", 8, 8);
			l1 = Lore.Literal($"{I18N.GetText("ethla:text.multiplier")} {GetToolEfficiency(stack)}");
			l0.Combine(l1);
			tooltips.Add(l0);
		}

		//rarity display
		int r = GetRarity(stack);
		l0 = Lore.Iconic($"ethla:image/gui/tooltip/item_rarity_{r}.png", 8, 8);
		l1 = Lore.Literal($"{I18N.GetText("ethla:text.rarity")} {NumberForm.GetRoman(r + 1)}");
		if (r <= 2)
			l1.Paint(0.85f, 0.92f, 1.00f);
		else if (r >= 3)
			l1.Paint(0.9f, 0.78f, 0.34f);
		l0.Combine(l1);
		tooltips.Add(l0);
	}

	public virtual InterResult OnClickBlock(ItemStack stack, Entity entity, Level level, Pos pos, bool sim = false)
	{
		return InterResult.Pass;
	}

	public virtual bool IsRemoteUsage(ItemStack stack)
	{
		return Property.RemoteUsage;
	}

	public virtual int GetRarity(ItemStack stack)
	{
		return Property.Rarity;
	}

	public virtual InterResult OnUseItem(ItemStack stack, Entity entity, Level level, Pos pos, bool sim = false)
	{
		return InterResult.Pass;
	}

	//P.S.:Only work with items whose MaxStackSize == 1.
	public virtual ItemStack OnStackInteracting(ItemStack stack, ItemStack interactor)
	{
		return interactor;
	}

}
