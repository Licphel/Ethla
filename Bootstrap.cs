using Ethla.Api;
using Ethla.Api.Scripting;
using Ethla.Client;
using Ethla.Client.Ui;
using Ethla.Common;
using Labyrinth;
using Microsoft.Win32;
using Spectrum.Audio;
using Spectrum.Core;
using Spectrum.Core.Input;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.IO;
using Spectrum.IO.Bin;
using Spectrum.IO.Pathfind;
using Spectrum.Maths;
using Spectrum.Native.XGLFW;
using Spectrum.Native.XOpenAL;
using Spectrum.Native.XOpenGL;
using Spectrum.Utils;

namespace Ethla;

public class Bootstrap
{

	public static readonly string DisName = "Ethla";
	public static readonly string Space = "ethla";
	public static readonly SemanticVersion Version = new SemanticVersion("alpha-1.0.0");

	public static PathHandle RootPath;
	public static PathHandle SavePath;
	public static PathHandle ContentPath;
	public static PathHandle OptionPath;
	public static PathHandle ModsPath;

	public static CallSequence IngameDraw = new CallSequence();
	public static CallSequence IngameTick = new CallSequence();
	public static CallSequence LoadDone = new CallSequence();

	public static bool IsIngameGuiOpened;
	public static TransformResolved TransformGui;
	public static TransformWorld TransformWorld = new TransformWorld();
	public static TransformAbsolute TransformScreen = new TransformAbsolute();
	public static Font FontOverall;

	public static void Launch()
	{
		Id.Converter = ModAssembly.GetFile;

		RootPath = PathRoot.ByExecution();
		ContentPath = RootPath.Find("content");
		OptionPath = RootPath.Find("options.lab");
		ModsPath = RootPath.Find("mod");
		SavePath = RootPath.Find("save");

		Options.Load();

		Logger.StartStreamWriting(RootPath.Find("latest.log"));

		GlfwAttribs s = new GlfwAttribs();
		s.Maximized = Options.Maximized;
		s.Size = new Vector2(400, 225);
		s.WindowSize = new Vector2(800, 450);
		s.Title = Options.Title;
		s.AutoIconify = Options.AutoIconify;
		s.Cursor = ContentPath.Find(Options.CursorLoc);
		s.Icons = [ContentPath.Find(Options.IconLoc)];
		s.Hotspot = new Vector2(Options.Hotspot.Get<float>(0), Options.Hotspot.Get<float>(1));

		GlfwHandle.SetGlfwConfiguration(s);
		GlfwHandle.MakeGlfwContext();
		Surface.Current = new OglSurface();
		OglInit.Init();
		AudioDevice.Current = new OalAudioDevice();

		TransformGui = new TransformResolved();

		Graphics graphics = Graphics.Global;

		Application.Tick.Add("ethla:tick", () =>
		{
			Keyboard.Global.StartRoll();

			Main.Tick();
			IngameTick?.Call();

			for (int i = 0; i < Gui.Viewings.Count; i++)
			{
				Gui gui = Gui.Viewings[i];
				gui.Tick(TransformGui.Cursor);
			}

			Keyboard.Global.EndRoll();
		});

		Application.Draw.Add("ethla:draw", () =>
		{
			graphics.Clear();
			Main.Draw(graphics);
			IngameDraw?.Call();
			graphics.Flush();

			for (int i = 0; i < Gui.Viewings.Count; i++)
			{
				Gui gui = Gui.Viewings[i];
				TransformGui.DoTransform(gui, graphics);
				graphics.UseCamera(TransformGui.Camera);

				if (gui != null)
				{
					//keep the font scale still.
					float fm = gui.FontScmul;
					if (graphics.Font != null) graphics.Font.Scale = fm;
					gui.Draw(graphics);
					if (graphics.Font != null) graphics.Font.Scale = 1;
				}

				graphics.Flush();
			}

			graphics.Flush();

			GlfwHandle.SwapBuffers();
			GlfwHandle.PrintErrs();
		});

		Application.Init.Add("ethla:init", () =>
		{
			LoadingQueue loader = LoadingManager.GetUploadQueueWithProcessors(Space, true);
			LoadingManager.LoadFontMaps();
			LoadingManager.LoadContents(loader, ContentPath);

			ExecutionResult result = ExecutionResult.From(new TextAccess(RootPath.Find("preloads.lab")));
			BinaryList preloadList = result.Result<BinaryList>();
			foreach (string pth in preloadList)
				loader.Scan(ContentPath.Find(pth), true);
			loader.DoPreLoad();

			Gui.DefaultTooltipPatches = new Slice9(Loads.Get("ethla:image/gui/tooltip.png"));

			Require.LoadClass(typeof(BlockMaterials));
			Require.LoadClass(typeof(Blocks));
			Require.LoadClass(typeof(Liquids));
			Require.LoadClass(typeof(Walls));
			Require.LoadClass(typeof(Items));
			Require.LoadClass(typeof(Features));
			Require.LoadClass(typeof(Biomes));
			Require.LoadClass(typeof(Prefabs));
			Require.LoadClass(typeof(Recipes));
			Require.LoadClass(typeof(SpawnEntries));
			Require.LoadClass(typeof(DamageTypes));

			ModAssembly.ScanLoad(ModsPath);
			ModAssembly.InitMods(loader);

			loader.Enqueue(() => { Require.LoadClass(typeof(Groups)); });

			new GuiLoading(loader).Display();
		});

		Application.Dispose.Add("ethla:dispose", () =>
		{
			Main.Level?.Save();
			Coroutine.Wait();
			Logger.TryEndStreamWriting();
		});

		Application.MaxTps = 60;
		Application.MaxFps = -1;
		Application.Launch();
	}

}
