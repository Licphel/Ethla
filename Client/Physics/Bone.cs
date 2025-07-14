using Ethla.Client.Iteming;
using Ethla.World.Iteming;
using Ethla.World.Physics;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;

namespace Ethla.Client.Physics;

public class Bone
{

	protected float AnchorX, AnchorY;
	protected bool Flip;
	protected ItemStack Held = ItemStack.Empty;
	protected Icon Icon;

	protected float Px, Py;
	protected float Swing;
	protected float W, H;

	public Bone SetIcon(Icon icon)
	{
		Icon = icon;
		return this;
	}

	public Bone Anchor(float x, float y)
	{
		AnchorX = x;
		AnchorY = y;
		return this;
	}

	public Bone Resize(float w, float h)
	{
		W = w;
		H = h;
		return this;
	}

	public void SetSwing(float swing)
	{
		Swing = swing;
	}

	public void SetHeldItemStack(ItemStack held)
	{
		Held = held;
	}

	public virtual void FindPosRelatively(Entity entity)
	{
		Flip = entity.Face.X < 0;
	}

	public void Draw(Graphics graphics, Entity entity, float x, float y)
	{
		TransformStack transformStack = graphics.TransformStack;

		if (!Held.IsEmpty)
		{
			transformStack.Push();
			transformStack.Translate(Px / 16f, Py / 16f);
			transformStack.Rotate(Rotation.Get(x + AnchorX / 16f, y + AnchorY / 16f, Angle.Degree(Flip ? 180 - Swing : Swing)));
			transformStack.Translate(AnchorX / 16f + (W - 2) / 16f, AnchorY / 16f - 0.25f);
			ItemModels.GetRenderer(Held).Draw(graphics, x, y, 0.5f, 0.5f, Held, false);
			transformStack.Pop();
		}

		transformStack.Push();
		transformStack.Translate(Px / 16f, Py / 16f);
		transformStack.Rotate(Rotation.Get(x + AnchorX / 16f, y + AnchorY / 16f, Angle.Degree(Flip ? 180 - Swing : Swing)));
		graphics.DrawIcon(Icon, x, y, W / 16f, H / 16f);
		transformStack.Pop();
	}

}
