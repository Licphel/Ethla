using Spectrum.Core.Manage;
using Spectrum.Graphic.Images;

namespace Ethla.Client;

public class Images
{

	public static Image ParticleBreakblock = Loads.Get("ethla:image/particle/break.png");
	public static Image ParticleSmoke = Loads.Get("ethla:image/particle/smoke.png");

	public static Image EntityOrb = Loads.Get("ethla:image/entity/magic_orb.png");
	public static Image EntityArrow = Loads.Get("ethla:image/entity/arrow.png");
	public static Image EntityDrone = Loads.Get("ethla:image/entity/drone.png");
	public static Image EntityDroneAncient = Loads.Get("ethla:image/entity/drone_ancient.png");
	public static Image EntityPlayer = Image.GetOverlayFromDirectory("ethla:image/entity/biped/style_0");
	public static Image EntityWorm = Loads.Get("ethla:image/entity/worm.png");

	public static Image PtkRain = Loads.Get("ethla:image/particle/rain.png");
	public static Image PtkSnow = Loads.Get("ethla:image/particle/snow.png");

	public static Image CeBody1 = Loads.Get("ethla:image/ambient/body_1.png");
	public static Image CeBody2 = Loads.Get("ethla:image/ambient/body_2.png");
	public static Image CeBg1 = Loads.Get("ethla:image/ambient/background_1.png");
	public static Image CeBg2 = Loads.Get("ethla:image/ambient/background_2.png");
	public static Image CeBg3 = Loads.Get("ethla:image/ambient/background_4.png");
	public static Image UCeBg1 = Loads.Get("ethla:image/ambient/underground_1.png");
	public static Image UCeBg2 = Loads.Get("ethla:image/ambient/underground_2.png");

	public static Image SlotChosen = Loads.Get("ethla:image/gui/slot.png");
	public static Image Button = Loads.Get("ethla:image/gui/button.png");
	public static Slice9 ButtonP1 = new Slice9(Button.Subimage(0, 0, 12, 12));
	public static Slice9 ButtonP2 = new Slice9(Button.Subimage(0, 12, 12, 12));
	public static Slice9 ButtonP3 = new Slice9(Button.Subimage(0, 24, 12, 12));
	public static Image Hotbar = Loads.Get("ethla:image/gui/hotbar.png");
	public static Image RecipeEntry = Loads.Get("ethla:image/gui/containers/recipe_entry.png");
	public static Image GuiInventory = Loads.Get("ethla:image/gui/containers/inventory.png");
	public static Image GuiChest = Loads.Get("ethla:image/gui/containers/chest.png");
	public static Image GuiFurnace = Loads.Get("ethla:image/gui/containers/furnace.png");

	public static Image GuiMenuTitle = Loads.Get("ethla:image/gui/menu.png");
	public static Image GuiMenuTitleCn = Loads.Get("ethla:image/gui/menu_chinese.png");

	public static Image IovWallMark = Loads.Get("ethla:image/item/overlay/wall_mark.png");

}
