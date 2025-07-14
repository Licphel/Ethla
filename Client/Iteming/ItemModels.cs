using Ethla.Common.Iteming;
using Ethla.World.Iteming;
using Spectrum.Core.Manage;
using Spectrum.Graphic.Images;

namespace Ethla.Client.Iteming;

public class ItemModels
{

	public static ItemRenderer GetRenderer(ItemStack stack)
	{
		if (stack.Item is ItemWand)
			return ItemRenderer.Wand;
		return ItemRenderer.Normal;
	}

	public static Icon GetIcon(ItemStack stack)
	{
		string appx = $"{stack.Item.Uid.Space}:image/item/{stack.Item.Uid.Key}";
		Id basicPth = new Id($"{appx}");
		return Loads.Get($"{appx}.png");
	}

}
