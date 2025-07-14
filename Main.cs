using System.Security.Cryptography.X509Certificates;
using Ethla.Client;
using Ethla.Client.Ambient;
using Ethla.Client.Audio;
using Ethla.Client.Messaging;
using Ethla.Client.Physics;
using Ethla.Common;
using Ethla.Common.Generating;
using Ethla.Common.Iteming;
using Ethla.Common.Mob;
using Ethla.Util;
using Ethla.World;
using Ethla.World.Generating;
using Ethla.World.Iteming;
using Ethla.World.Mob;
using Ethla.World.Mob.Ai;
using Ethla.World.Physics;
using Ethla.World.Voxel;
using Spectrum.Codec;
using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.IO.Bin;
using Spectrum.IO.Pathfind;
using Spectrum.Maths;
using Spectrum.Maths.Random;
using Spectrum.Maths.Shapes;
using Spectrum.Native.XOpenGL;

namespace Ethla;

public class Main
{

	public static Camera Camera = new Camera();
	public static float Cdx, Cdy;
	public static float Cx, Cy;
	private static float nowScale = 0.05f;
	public static float Pcdx, Pcdy;
	public static float Pcx, Pcy;
	private static float pScale = 0.05f;
	private static float sAcc;
	private static float sDir;
	public static Quad Viewdim;

	public static BlockPos BreakPos;
	public static float BreakProgress;
	private static float digsoundBuf;
	public static float Hdnow;

	public static bool HoverBlocked;
	public static BlockState HoverBlockState = BlockState.Empty;
	public static BlockPos HoverPos;
	public static PrecisePos HoverPosPrecise;
	private static float usebuf;
	public static Entity FirstHoverEntity;

	public static Level Level;
	public static MusicPlayer MusicPlayer = new MusicPlayer();
	public static EntityPlayer Player;
	public static PathHandle PlayerSave = Bootstrap.RootPath.Find("save/player.bin");

	public static Sky Sky;

	public static bool Pausing => Bootstrap.IsIngameGuiOpened;

	public static bool GodMode;

	public static void Load()
	{
		Level = new Level(new LevelType("ethla:overworld", level =>
		{
			Generator generator = new GeneratorImpl(level);
			generator.AddDecorator(new DecoratorCave());
			generator.AddDecorator(new DecoratorPlants());
			generator.AddProcessor(new PostProcessorFeatures());
			return generator;
		}));
		Level.TryLoad();

		Sky = new Sky(Level);

		if (PlayerSave.Exists)
		{
			BinaryCompound compound = BinaryRw.Read(PlayerSave);
			Player = (EntityPlayer)Entity.ReadFromType(compound);
		}

		if (Player == null)
		{
			Player = Prefabs.Player.Instantiate();
			Player.Locate(new PrecisePos(0, 128), false);
			Player.Inventory.Add(Items.IronAxe.MakeStack());
			Player.Inventory.Add(Items.IronPickaxe.MakeStack());
			Player.Inventory.Add(Items.IronShovel.MakeStack());
			Player.Inventory.Add(Items.IronHammer.MakeStack());
			Player.Inventory.Add(Blocks.Torch.Item.MakeStack(30));
		}

		Level.Generator.Provide(Player.Pos.UnitX);
		Player.IsPlayerControl = true;
		Player.Renderer = new PlayerRenderer(Images.EntityPlayer);
		Level.SpawnEntity(Player);

		Cx = Player.Pos.X;
		Cy = Player.Pos.Y;
	}

	public static void SavePlayer()
	{
		BinaryCompound compound = BinaryCompound.New();
		Player.Write(compound);
		BinaryRw.Write(compound, PlayerSave);
	}

	public static void Tick()
	{
		if (Level == null) return;

		Player.IgnoreLandFriction = false;
		MusicPlayer.Tick();

		if (!Pausing)
		{
			const float j = 4.97f, fl = 11.8f;
			float a = 9.5f * Time.Delta, s = 6f;

			//Player move
			if (KeyBind.KeyA.Holding())
			{
				MoveHandler.LerpedMove(Player, new Vector2(-s, 0), a);
				Player.IgnoreLandFriction = true;
			}

			if (KeyBind.KeyD.Holding())
			{
				MoveHandler.LerpedMove(Player, new Vector2(s, 0), a);
				Player.IgnoreLandFriction = true;
			}

			if (KeyBind.KeySpace.Pressed() && Player.TouchDown)
				Player.ApplyImpulse(new Vector2(0, j * Player.Mass));

			if ((KeyBind.KeySpace.Holding() || KeyBind.KeyW.Holding()) && Player.IsFloating)
				Player.ApplyImpulse(new Vector2(0, fl * Player.Mass * Time.Delta));

			if (KeyBind.KeyS.Holding() && Player.IsFloating)
				Player.ApplyImpulse(new Vector2(0, -fl * Player.Mass * Time.Delta));

			//PLY
			float cmx = Bootstrap.TransformWorld.Cursor.X;
			float cmy = Bootstrap.TransformWorld.Cursor.Y;

			HoverPosPrecise = new PrecisePos(cmx, cmy);
			HoverPos = new BlockPos(Mathf.FastFloor(cmx), Mathf.FastFloor(cmy));
			HoverBlockState = Level.GetBlock(HoverPos);

			HoverBlocked = Ray.IsBlocked(Level, Player.Pos, HoverPos, 6f, 2) && !GodMode;

			checkUsing();

			//Open inventory
			if (KeyBind.KeyF.Pressed())
			{
				SlotLayoutInventory c = new SlotLayoutInventory(Player);
				SlotLayout.OpenGracefully(Player, c);
			}

			if (KeyBind.KeyCtrl.Holding()) Player.Locate(HoverPos);
			if (KeyBind.KeyF2.Pressed()) GodMode = !GodMode;
			if (KeyBind.KeyF3.Holding()) Level.Ticks += 10;

			Player.Face.X = cmx < Player.Pos.X ? -1 : 1;

			listenScrollEvent();
		}

		tickCamera();

		Level.LightEngine.CalculateByViewdim(Viewdim);
		Level.Tick();
		Sky.Tick(Player.Pos);
		digsoundBuf -= Time.Delta;
		usebuf--;

		if (Player.IsDead && Player.DeathTimer >= 1)
		{
			Player.IsDead = false;
			Player.DeathTimer = 0;
			Player.Health = Player.MaxHealth;
			Player.Mana = Player.MaxMana;
			Player.Hunger = Player.MaxHunger;
			Player.Thirst = Player.MaxThirst;
			Player.Locate(0, 128);
			Level.Generator.Provide(Player.Pos.UnitX);
			Level.SpawnEntity(Player);
			MessageManager.Send(new Message(Player.CausingDamage.GetMurderInfo()));
		}
	}

	public static void Draw(Graphics graphics)
	{
		if (Level == null) return;

		float x1 = Time.Lerp(Pcdx, Cdx) + Time.Lerp(Pcx, Cx);
		float y1 = Time.Lerp(Pcdy, Cdy) + Time.Lerp(Pcy, Cy);

		Sky.CenterMoved(x1, y1 - Chunk.YOfSea - 15);

		float w = Camera.Viewport.W * Camera.ScaleX;
		float h = Camera.Viewport.H * Camera.ScaleY;

		if (float.IsNaN(w) || float.IsNaN(h)) return;

		if (y1 + h / 2f >= Chunk.Height) y1 = Chunk.Height - h / 2f;
		if (y1 - h / 2f <= 0) y1 = h / 2f; //limit

		Viewdim.Resize(w, h);
		Viewdim.LocateCentral(x1, y1);

		Camera.Center.X = x1;
		Camera.Center.Y = y1;
		Camera.ScaleX = Camera.ScaleY = Time.Lerp(pScale, nowScale);
		Camera.Push();

		Bootstrap.TransformScreen.DoTransform(graphics);
		graphics.UseCamera(Bootstrap.TransformScreen.Camera);
		Sky.Draw(graphics, Level.Ticks, Level.TicksPerDay, Player.Pos);

		Bootstrap.TransformWorld.DoTransform(graphics, Camera);
		graphics.UseCamera(Camera);

		WorldRenderer.Draw(graphics, Level, Viewdim);

		if (BreakProgress > 0)
		{
			int brx = (int)(BreakProgress / Hdnow * 8) * 16;
			graphics.DrawImage(Images.ParticleBreakblock, BreakPos.BlockX, BreakPos.BlockY, 1, 1, brx, 0, 16, 16);
		}

		if (!HoverBlockState.IsEmpty && !HoverBlocked && FirstHoverEntity == null)
		{
			graphics.Color4(0.95f, 0.95f, 1f, 0.25f);
			graphics.DrawRectOutline(HoverPos.BlockX, HoverPos.BlockY, 1, 1);
			graphics.NormalizeColor();
		}

		if (FirstHoverEntity != null)
		{
			graphics.Color4(0.95f, 0.95f, 1f, 0.25f);
			graphics.DrawRectOutline(FirstHoverEntity.Bound);
			graphics.NormalizeColor();
		}

		graphics.UseCamera(Camera.Absolute);
	}

	private static void listenScrollEvent()
	{
		int slot = Player.InvCursor;

		switch (Keyboard.Global.ScrollDirection)
		{
			case ScrollDirection.Down:
				Player.InvCursor = slot + 1;
				Keyboard.Global.ConsumeCursorScroll();
				break;
			case ScrollDirection.Up:
				Player.InvCursor = slot - 1;
				Keyboard.Global.ConsumeCursorScroll();
				break;
		}

		if (Player.InvCursor == 10) Player.InvCursor = 0;
		if (Player.InvCursor == -1) Player.InvCursor = 9;
	}

	private static void tickCamera()
	{
		bool smv = false;

		if (KeyBind.KeyX.Holding() && !Pausing)
		{
			sAcc = 0.5f * nowScale;
			sDir = 1;
			smv = true;
		}

		if (KeyBind.KeyZ.Holding() && !Pausing)
		{
			sAcc = 0.4f * nowScale;
			sDir = -1;
			smv = true;
		}

		if (!smv) sAcc *= 0.9f;

		pScale = nowScale;
		nowScale += sAcc * sDir * 0.05f;
		float l1 = 6 / Camera.Viewport.W;
		float l2 = 40 / Camera.Viewport.H;
		pScale = Math.Clamp(pScale, l1, l2);
		nowScale = Math.Clamp(nowScale, l1, l2);

		Pcdx = Cdx;
		Pcdy = Cdy;
		Pcx = Cx;
		Pcy = Cy;

		ImVector2 cursor = Bootstrap.TransformGui.Cursor;
		float dx = cursor.X - Surface.Current.Size.X / 2f;
		float dy = cursor.Y - Surface.Current.Size.Y / 2f;

		float dt = 0.015f; //a %.2f limit so try div it
		float dl = 0.1f;

		//check to keep validity while back running
		if (!float.IsNaN(dx) && !float.IsNaN(dy) && !float.IsNaN(dl) && !Pausing)
		{
			dx *= dt / 16f;
			dy *= dt / 16f;
			Cdx += (dx - Cdx) * dl;
			Cdy += (dy - Cdy) * dl;
		}

		Cx += (Player.Pos.X - Cx) * dl;
		Cy += (Player.Pos.Y - Cy) * dl;
	}

	private static void checkUsing()
	{
		//Use Entities
		if (!HoverBlocked)
		{
			Quad box = new Quad();
			box.SetCentral(HoverPosPrecise.X, HoverPosPrecise.Y, 0.05f, 0.05f);
			List<Entity> entities = Level.GetNearbyEntities(box);

			if (KeyBind.MouseRight.Holding() && usebuf <= 0)
			{
				foreach (Entity e in entities)
				{
					if (e.OnInteract(Player, HoverPosPrecise) == InterResult.Blocked)
					{
						usebuf = 2;
						break;
					}
				}
			}

			FirstHoverEntity = null;

			if (entities.Count > 0 && !HoverBlocked)
				FirstHoverEntity = entities[0];

			if (FirstHoverEntity == Player)
				FirstHoverEntity = null;

			if (FirstHoverEntity != null && KeyBind.MouseLeft.Pressed())
			{
				if (FirstHoverEntity is Creature creature)
				{
					creature.Hit(new Damage(Player, DamageTypes.Mechanic, 2.5f));
				}
			}
		}

		bool blocked = HoverBlocked || FirstHoverEntity != null;

		if (blocked)
		{
			BreakProgress = 0;
		}

		//Use items or click blocks
		if (KeyBind.MouseRight.Holding() && usebuf <= 0)
		{
			ItemStack stack = Player.Inventory.Get(Player.InvCursor);

			usebuf = 2;
			InterResult ir = InterResult.Pass;

			if (!blocked)
			{
				ir = stack.OnClickBlock(Player, Level, HoverPos);
			}

			if (ir == InterResult.Pass)
			{
				if (!blocked && KeyBind.MouseRight.Pressed() && HoverBlockState.HasSlotLayout(HoverPos, Player))
				{
					SlotLayout cab = HoverBlockState.CreateSlotLayout(HoverPos, Player);
					SlotLayout.OpenGracefully(Player, cab);
				}
				else if (stack.IsRemoteUsage())
				{
					stack.OnUseItem(Player, Level, new PrecisePos(Bootstrap.TransformWorld.Cursor.X, Bootstrap.TransformWorld.Cursor.Y));
				}
			}

		}

		//Break blocks
		if (!blocked && KeyBind.MouseLeft.Holding() && usebuf <= 0)
		{
			if (BreakPos != HoverPos)
				BreakProgress = 0;
			BreakPos = HoverPos;
			ItemStack held = Player.Inventory[Player.InvCursor];
			ItemToolType toolType = held.GetToolType();
			float boost = GodMode ? float.MaxValue : held.GetToolEfficiency();

			if (toolType.ShouldHarvestWall())
			{
				Wall wall = Level.GetWall(BreakPos);

				if (wall != Wall.Empty)
				{
					Hdnow = wall.GetHardness();

					if (digsoundBuf <= 0) digsoundBuf = (float)wall.GetSound("dig").PlaySound(HoverPos).TotalSeconds;

					BreakProgress += Time.Delta * boost;

					if (TimeSchedule.PeriodicTask(0.1f)) Level.PlayDestructParticles(wall, HoverPos, 1);

					if (BreakProgress >= Hdnow && Hdnow > 0)
					{
						Level.BreakWall(BreakPos, Player);
						BreakProgress = 0;
					}
				}
				else
				{
					BreakProgress = 0;
				}
			}
			else
			{
				BlockState state = Level.GetBlock(BreakPos);

				if (!state.IsEmpty)
				{
					Hdnow = state.GetHardness();

					bool boostapply = toolType.IsTarget(state.GetMaterial()) || GodMode;

					if (digsoundBuf <= 0) digsoundBuf = (float)state.GetSound("dig").PlaySound(HoverPos).TotalSeconds;

					BreakProgress += Time.Delta * (boostapply ? boost : 1);

					if (TimeSchedule.PeriodicTask(0.1f)) Level.PlayDestructParticles(state, HoverPos, 1);

					if (BreakProgress >= Hdnow && Hdnow > 0)
					{
						Level.BreakBlock(BreakPos, Player);
						BreakProgress = 0;
					}
				}
				else
				{
					BreakProgress = 0;
				}
			}
		}
		else
		{
			BreakProgress = 0;
		}
	}

}
