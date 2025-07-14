using Ethla.Client.Ui;
using Ethla.Common.Mob;
using Ethla.Common.Voxel;
using Ethla.World;
using Ethla.World.Iteming;
using Spectrum.Graphic.Ui;

namespace Ethla.Common.Iteming;

public class SlotLayoutFurnace : SlotLayout
{

	public BlockEntityFurnace Furnace;

	public SlotLayoutFurnace(BlockPos pos, EntityPlayer player) : base(player)
	{
		Furnace = (BlockEntityFurnace)Level.GetBlockEntity(pos);
		Invs.Add(Furnace.Inv);

		//#? player inv bind slots
		AddPlayerSide(5, 5);
		//#0 cab inv bind slots
		Carve(new Slot(0, 68, 136, false, true));
		Carve(new Slot(1, 68 + 21, 136, false, true));
		Carve(new Slot(2, 68 + 42, 136, false, true));
		Carve(new Slot(3, 26, 136, false, true));
		Carve(new Slot(4, 173, 136, false, true));
	}

	public override Gui MakeScreen()
	{
		return new GuiFurnace(this);
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
			if (clicked.Stack.Is(Groups.Fuel))
				invPlayer[i] = Invs[0].Add(invPlayer[i], 3, 3);
			else
				invPlayer[i] = Invs[0].Add(invPlayer[i], 0, 2);
		}
	}

}
