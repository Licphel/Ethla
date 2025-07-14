using Ethla.Client;
using Ethla.Client.Ui;
using Ethla.Common.Mob;
using Ethla.Common.Voxel;
using Ethla.World;
using Ethla.World.Iteming;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Common.Iteming;

public class SlotLayoutChest : SlotLayout
{

	public SlotLayoutChest(BlockPos pos, EntityPlayer player) : base(player)
	{
		BlockEntityChest chest = (BlockEntityChest)Level.GetBlockEntity(pos);
		Invs.Add(chest.Inv);

		//#? player inv bind slots
		AddPlayerSide(5, 5);
		//#0 cab inv bind slots
		Add3X10Side(5, 101, 0, false);
	}

	public override Gui MakeScreen()
	{
		return new GuiSlotLayout(this, Images.GuiChest, new Vector2(219, 183), Lore.Translate("ethla:block.chest"));
	}

	public override void Transport(Slot clicked, ItemContainer clickedInv, Inventory invPlayer)
	{
		int i = clicked.InvIndex;

		if (!clicked.PlayerSide)
			clickedInv[i] = invPlayer.Add(clickedInv[i]);
		else
			invPlayer[i] = Invs[0].Add(invPlayer[i]);
	}

}
