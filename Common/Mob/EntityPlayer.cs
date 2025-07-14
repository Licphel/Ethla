using Ethla.Client.Physics;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Spectrum.Graphic;
using Spectrum.IO.Bin;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityPlayer : Creature
{

	private static readonly int LoadArea = 3;
	public ItemStack GuiHeldStack = ItemStack.Empty;

	public bool IsPlayerControl;
	public SlotLayout OpenContainer;

	public PlayerRenderer Renderer;

	public EntityPlayer()
	{
		Inventory = new Inventory(40);
		Health = MaxHealth = 100;
		Mana = MaxMana = 100;
		Hunger = MaxHunger = 100;
		Thirst = MaxThirst = 100;
	}

	public override void Tick()
	{
		base.Tick();

		Move();

		Quad box2 = Bound;
		box2.Scale(2f, 2f);
		List<Entity> lst = Level.GetNearbyEntities(box2, typeof(EntityItem));

		foreach (Entity e in lst)
		{
			EntityItem e1 = (EntityItem)e;

			if (e1.Protection > 0)
				continue;

			Quad buf = Bound;
			buf.Scale(0.5f, 0.5f);

			if (buf.Interacts(e.Bound))
			{
				e1.Pickup(this, Inventory);
			}
			else
			{
				ItemStack stack = e1.Stack;

				if (!Inventory.Add(stack.Copy(), true).Is(stack))
				{
					float spd = 2f / Posing.Distance(e.Pos, Pos) + e.Velocity.Len;
					float rad = Posing.PointRad(e.Pos, Pos);
					e.Velocity.X = spd * Mathf.CosRad(rad);
					e.Velocity.Y = spd * Mathf.SinRad(rad);
				}
			}
		}

		TickLoadingChunks(Level, ChunkUnit.Pos.UnitX);
	}

	public override void OnHit(Damage value)
	{
		if (Main.GodMode)
			Health = MaxHealth;
	}

	public override void Write(BinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("guistack", ItemStack.Serialize(GuiHeldStack));
		Inventory.Serialize(Inventory, compound.NewScope("pinv"));
		compound.Set("last_level", Level.Type.Id.ToString());
	}

	public override void Read(BinaryCompound compound)
	{
		base.Read(compound);
		GuiHeldStack = ItemStack.Deserialize(compound.GetCompoundSafely("guistack"));
		Inventory = Inventory.Deserialize(compound.GetCompoundSafely("pinv"));
	}

	public void TickLoadingChunks(Level level, int sx)
	{
		if (!IsPlayerControl)
			return;

		for (int i = sx - LoadArea - 1; i < sx + LoadArea + 1; i++)
		{
			level.Generator.ProvideAsync(i);
			Chunk chunk = level.GetChunk(i);
			if (chunk == null)
				continue;
			chunk.RefreshTime = Chunk.RefreshTimeNormal;
		}
	}

	public override float CastLight(byte pipe)
	{
		return Math.Max(0.005f, base.CastLight(pipe));
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		Renderer.Draw(graphics, this, rect);
	}

}
