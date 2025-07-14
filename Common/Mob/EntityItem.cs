using Ethla.Client.Iteming;
using Ethla.World;
using Ethla.World.Iteming;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.IO.Bin;
using Spectrum.Maths.Random;
using Spectrum.Maths.Shapes;

namespace Ethla.Common.Mob;

public class EntityItem : Entity
{

	public float Protection;

	public ItemStack Stack = ItemStack.Empty;

	public static void PopDrop(Level level, List<ItemStack> stacks, Pos pos)
	{
		foreach (ItemStack stack in stacks)
		{
			if (stack.IsEmpty)
				return;
			EntityItem itemEntity = Prefabs.Item.Instantiate();
			itemEntity.Stack = stack;
			float rx = Seed.Global.NextFloat(-0.25f, 0.25f);
			float ry = Seed.Global.NextFloat(-0.25f, 0.25f);
			itemEntity.Velocity.X = Seed.Global.NextFloat(-2f, 2f);
			itemEntity.Velocity.Y = Seed.Global.NextFloat(-2f, 2f);
			level.SpawnEntity(itemEntity, pos.X + rx, pos.Y + ry);
		}
	}

	public override void Tick()
	{
		base.Tick();

		Move();

		if (Protection <= 0 && TimeSchedule.PeriodicTask(LiveTime, 2))
		{
			Quad box2 = Bound;
			box2.Expand(1, 1);
			List<Entity> lst = Level.GetNearbyEntities(box2, typeof(EntityItem));
			lst.ForEach(e => absorb((EntityItem)e));
		}

		Protection -= Time.Delta;
	}

	private void absorb(EntityItem e)
	{
		if (e == this) return;

		if (Stack.Count >= Stack.MaxStackSize) return;

		ItemStack i1 = e.Stack;

		if (i1.Count > Stack.Count || i1.IsEmpty) return;

		if (Stack.CanMergePartly(i1))
		{
			ItemStack remain = Stack.Merge(i1);
			e.Stack = remain;

			if (e.Stack.IsEmpty) e.Die();
		}
	}

	//wonn't be fully picked up sometimes.
	public void Pickup(Entity entity, Inventory inv)
	{
		ItemStack trystack = inv.Add(Stack.Copy());
		if (trystack.IsEmpty)
			Die();
		Stack = trystack;
	}

	public override float CastLight(byte pipe)
	{
		return Stack.CastLight(pipe);
	}

	public override void Write(BinaryCompound compound)
	{
		base.Write(compound);
		compound.Set("stack", ItemStack.Serialize(Stack));
	}

	public override void Read(BinaryCompound compound)
	{
		base.Read(compound);
		Stack = ItemStack.Deserialize(compound.GetCompoundSafely("stack"));
	}

	public override void Draw(Graphics graphics, Quad rect)
	{
		graphics.Color4(Light);
		ItemModels.GetRenderer(Stack).Draw(graphics, rect, Stack, false);
		graphics.NormalizeColor();
	}

}
