using Ethla.Common;
using Ethla.Common.Mob;
using Spectrum.Graphic.Ui;

namespace Ethla.World.Iteming;

public class SlotLayout
{

	public Slot ChosenSlot;
	public List<Inventory> Invs = new List<Inventory>();

	public Level Level;
	public EntityPlayer Player;
	public List<Slot> Slots = new List<Slot>();

	public SlotLayout(EntityPlayer player)
	{
		Player = player;
		Level = player.Level;
	}

	public ItemStack Pickup
	{
		get => Player.GuiHeldStack;
		set => Player.GuiHeldStack = value;
	}

	//UTILS

	public void OnStoppedUsage()
	{
		DoAction(SlotAction.ThrowAll);

		foreach (Slot slot in Slots)
			if (!slot.Storage)
			{
				EntityItem entity = Prefabs.Item.Instantiate();
				entity.Stack = slot.Stack;
				Level.SpawnEntity(entity, Player.Pos);
			}
	}

	public void Carve(Slot slot, int invi = 0)
	{
		Slots.Add(slot);

		int uuid = Slots.IndexOf(slot);

		if (slot.PlayerSide)
			slot.Bind(Player.Inventory, uuid);
		else
			slot.Bind(Invs[invi], uuid);
	}

	public void DoAction(SlotAction action)
	{
		if (!Pickup.IsEmpty)
		{
			EntityItem entity = Prefabs.Item.Instantiate();
			bool thr = false;
			if (action == SlotAction.ThrowAll)
			{
				entity.Stack = Pickup;
				Pickup = ItemStack.Empty;
				thr = true;
			}
			else if (action == SlotAction.ThrowOne)
			{
				entity.Stack = Pickup.Take(1);
				thr = true;
			}
			if (thr)
			{
				entity.Protection = 5;
				Level.SpawnEntity(entity, Player.Pos);
				return;
			}
		}

		if (ChosenSlot == null) return;
		switch (action)
		{
			case SlotAction.SwapPickup:
				ItemStack pick0 = Pickup;

				ItemStack slot0 = ChosenSlot.Stack;
				if (pick0.CanMergePartly(slot0) && !slot0.IsEmpty && !pick0.IsEmpty)
				{
					Pickup = slot0.Merge(pick0);
				}
				else
				{
					ChosenSlot.Stack = pick0;
					Pickup = slot0;
				}

				break;
			case SlotAction.FastTransfer:
				Transport(ChosenSlot, ChosenSlot.Inv, Player.Inventory);
				break;
			case SlotAction.HalfPickup:
				pick0 = Pickup;
				slot0 = ChosenSlot.Stack;
				if (pick0.IsEmpty && !slot0.IsEmpty)
				{
					if (slot0.MaxStackSize == 1)
						Pickup = slot0.OnStackInteracting(pick0);

					//if the interaction on a one-stack
					if (Pickup.IsEmpty)
						Pickup = ChosenSlot.Stack.SpiltHalf();
				}
				else if (!pick0.IsEmpty)
				{
					if (slot0.IsEmpty)
						ChosenSlot.Stack = pick0.Take(1);
					else if (slot0.CanMergePartly(pick0))
						slot0.Merge(pick0.Take(1));
					else if (!slot0.IsEmpty && slot0.MaxStackSize == 1)
						Pickup = slot0.OnStackInteracting(pick0);
				}

				break;
		}
	}

	public virtual void Transport(Slot clicked, ItemContainer clickedInv, Inventory invPlayer)
	{
	}

	public virtual Gui MakeScreen()
	{
		return null;
	}

	public static void OpenGracefully(EntityPlayer player, SlotLayout ci)
	{
		player.OpenContainer = ci;
		ci.MakeScreen().Display();
		Bootstrap.IsIngameGuiOpened = true;
	}

	public static void CloseGracefully(EntityPlayer player)
	{
		player.OpenContainer?.OnStoppedUsage();
		player.OpenContainer = null;
		Bootstrap.IsIngameGuiOpened = false;
	}

	public void Add3X10Side(int x0 = 0, int y0 = 0, int invi = 0, bool playerside = true)
	{
		int sw = 20 + 1;

		int x = x0;
		int y = y0;

		for (int i = 0; i < 10; i++) Carve(new Slot(i, i * sw + x, y, playerside, true), invi);
		for (int i = 0; i < 10; i++) Carve(new Slot(i + 10, i * sw + x, y + sw, playerside, true), invi);
		for (int i = 0; i < 10; i++) Carve(new Slot(i + 20, i * sw + x, y + sw * 2, playerside, true), invi);
	}

	public void AddPlayerSide(int x0 = 0, int y0 = 0, int invi = 0, bool playerside = true)
	{
		int sw = 20 + 1;

		int x = x0;
		int y = y0;

		for (int i = 0; i < 10; i++) Carve(new Slot(i, i * sw + x, y, playerside, true), invi);
		for (int i = 0; i < 10; i++) Carve(new Slot(i + 10, i * sw + x, y + sw + 2, playerside, true), invi);
		for (int i = 0; i < 10; i++) Carve(new Slot(i + 20, i * sw + x, y + sw * 2 + 2, playerside, true), invi);
		for (int i = 0; i < 10; i++) Carve(new Slot(i + 30, i * sw + x, y + sw * 3 + 2, playerside, true), invi);
	}

	public void PlateCarve(int i0, int x0, int y0, int xn, int yn, bool playerside, bool store, bool oonly, int invi = 0)
	{
		int sw = 20 + 1;

		int x = x0;
		int y = y0;

		for (int j = 0; j < yn; j++)
			for (int i = 0; i < xn; i++)
				Carve(new Slot(i0 + i + j * xn, i * sw + x, j * (sw + 1) + y, playerside, store), invi);
	}

}
