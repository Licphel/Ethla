using Ethla.Common.Mob;
using Ethla.World.Physics;
using Spectrum.Core;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Maths;
using Spectrum.Maths.Shapes;

namespace Ethla.Client.Physics;

public class PlayerRenderer
{

	private readonly Animation flyDown;
	private readonly Animation flyUp;

	private readonly BoneGroup group;
	private readonly Bone handLeft;
	private readonly Bone handRight;

	private readonly Animation standing;
	private readonly Animation walking;
	private Image image;

	private float lastSwing;

	public PlayerRenderer(Image img)
	{
		image = img;
		standing = new Animation(img, 4, 16, 32, 0, 0).Seconds(0.25f);
		walking = new Animation(img, 6, 16, 32, 0, 32).Seconds(0.25f);
		flyUp = new Animation(img, 4, 16, 32, 0, 64).Seconds(0.25f);
		flyDown = new Animation(img, 4, 16, 32, 0, 96).Seconds(0.25f);
		handRight = getHand(img);
		handLeft = getHand(img);
		group = new BoneGroup();
		group.AddToBack(handLeft);
		group.AddToFront(handRight);
	}

	public void Draw(Graphics graphics, Entity entity, Quad rect)
	{
		EntityPlayer player = (EntityPlayer)entity;

		ImVector2 cursor = Bootstrap.TransformWorld.Cursor;

		float deg = player.IsPlayerControl ? Mathf.AtanDeg(-rect.Ycentral + cursor.Y, -rect.Xcentral + cursor.X) : -90;
		handRight.SetSwing(deg);
		handRight.SetHeldItemStack(player.Inventory[player.InvCursor]);

		float nowSwing = -90;
		handLeft.SetSwing(Time.Lerp(lastSwing, nowSwing));
		lastSwing = nowSwing;

		group.SetBody(GetAnim(player));

		graphics.Color4(entity.Light);
		if (player.HurtCooldown > 0)
			graphics.Merge4(1, 0.3f, 0.3f);
		graphics.Merge4(1, 1 - player.DeathTimer, 1 - player.DeathTimer, 1 - player.DeathTimer);
		TransformStack transformStack = graphics.TransformStack;
		transformStack.Push();
		transformStack.Translate(rect.Xcentral, rect.Ycentral);
		transformStack.Scale(entity.Face.X < 0 ? -1 : 1, 1);
		transformStack.Translate(-rect.Xcentral, -rect.Ycentral);
		BonedRendering.Draw(graphics, group, entity, rect);
		transformStack.Pop();

		graphics.NormalizeColor();
	}

	private static Bone getHand(Image img)
	{
		return new HandPlayer().SetIcon(img.Subimage(65, 2, 8, 3)).Anchor(0, 2).Resize(8, 3);
	}

	public Animation GetAnim(EntityPlayer entity)
	{
		Animation frame;

		if (entity.Velocity.Y > 0.01f && !entity.TouchUp)
			return flyUp;
		if (entity.Velocity.Y < -0.01f && !entity.TouchDown)
			return flyDown;

		if (Math.Abs(entity.Velocity.X) < 0.01f || entity.TouchLeft || entity.TouchRight)
			return standing;

		frame = walking;
		frame.Seconds(0.35f - Math.Min(0.27f, Math.Abs(entity.Velocity.X / 20f)));
		return frame;
	}

	private class HandPlayer : Bone
	{

		public override void FindPosRelatively(Entity entity)
		{
			base.FindPosRelatively(entity);
			Px = 5;
			Py = 14;
		}

	}

}
