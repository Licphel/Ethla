using Ethla.Client.Ui;
using Ethla.Common.Mob;
using Ethla.World.Iteming;
using Spectrum.Graphic.Ui;

namespace Ethla.Common.Iteming;

public class SlotLayoutInventory : SlotLayout
{

	public SlotLayoutInventory(EntityPlayer player) : base(player)
	{
		AddPlayerSide(5, 5);
	}

	public override Gui MakeScreen()
	{
		return new GuiInventory(this);
	}

	public override void Transport(Slot clicked, ItemContainer clickedInv, Inventory invPlayer)
	{
		int i = clicked.InvIndex;

		if (!clicked.PlayerSide)
		{
			clickedInv[i] = invPlayer.Add(clickedInv[i]);
		}
		else
		{
			if (i <= 8)
				invPlayer[i] = invPlayer.Add(invPlayer[i], 9, 26);
			else
				invPlayer[i] = invPlayer.Add(invPlayer[i], 0, 8);
		}
	}

}
