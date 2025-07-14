using Ethla.Api.Scripting;
using Ethla.Client.Audio;
using Labyrinth;
using Spectrum.Audio;
using Spectrum.Core.Manage;
using Spectrum.Graphic;
using Spectrum.Graphic.Images;
using Spectrum.Graphic.Text;
using Spectrum.IO;
using Spectrum.IO.Bin;
using Spectrum.IO.Pathfind;

namespace Ethla;

public class LoadingManager
{

	public static ImageBuffer BlockItemBuffer = new ImageBuffer();

	public static UploadQueueProcessor LangProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryCompound compound = result.Result<BinaryCompound>();

		I18N.Load(result.Space, file.Name, compound);
	};

	public static UploadQueueProcessor GroupProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		string space = result.Space;
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.AddGroup(id, result.Returner.Boxed);
	};

	public static UploadQueueProcessor TextureProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("png", StringComparison.OrdinalIgnoreCase))
			return;

		ImageSurface image = Surface.Current.NewImageSurface(file);
		dynamic finalsub = image;

		if (resource.Key.StartsWith("image/block") || resource.Key.StartsWith("image/item") || resource.Key.StartsWith("image/wall"))
			finalsub = BlockItemBuffer.Accept(image);

		PathHandle animpth = PathRoot.Of(file.Path + ".lab");
		if (animpth.Exists)
		{
			ExecutionResult result = ExecutionResult.From(new TextAccess(animpth));
			BinaryCompound compound = result.Result<BinaryCompound>();
			BinaryList ints = compound.GetListSafely("frame_uv");
			finalsub = new Animation(finalsub, compound.Get<int>("frame_count"), ints.Get<int>(2), ints.Get<int>(3), ints.Get<int>(0), ints.Get<int>(1)).Seconds(compound.Get<float>("frame_length"));
		}

		Loads.Load(resource, finalsub);
	};

	public static UploadQueueProcessor MusicSoundProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("wav", StringComparison.OrdinalIgnoreCase))
			return;

		TrackData data = AudioDevice.Current.NewTrackData(file);
		Loads.Load(resource, data);
		Music.Musics.Add(new Music(resource));
	};

	public static UploadQueueProcessor ShaderProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("shd", StringComparison.OrdinalIgnoreCase))
			return;

		string @string = StrRw.Read(file);
		Loads.Load(resource, @string);
	};

	public static UploadQueueProcessor RecipeProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		string space = result.Space;
		BinaryCompound compound = result.Result<BinaryCompound>();
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.AddRecipe(id, compound);
	};

	public static UploadQueueProcessor BlockModelProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryList list = result.Result<BinaryList>();
		ScriptDecoder.AddBlockModels(list);
	};

	public static UploadQueueProcessor WallModelProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryList list = result.Result<BinaryList>();
		ScriptDecoder.AddWallModels(list);
	};

	public static UploadQueueProcessor BlockPropertyProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryCompound compound = result.Result<BinaryCompound>();
		string space = result.Space;
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.SetBlockProperty(id, compound);
	};

	public static UploadQueueProcessor WallPropertyProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryCompound compound = result.Result<BinaryCompound>();
		string space = result.Space;
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.SetWallProperty(id, compound);
	};

	public static UploadQueueProcessor ItemPropertyProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryCompound compound = result.Result<BinaryCompound>();
		string space = result.Space;
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.SetItemProperty(id, compound);
	};

	public static UploadQueueProcessor PrefabPropertyProcessor = (resource, file) =>
	{
		if (!file.Format.Equals("lab", StringComparison.OrdinalIgnoreCase))
			return;

		ExecutionResult result = ExecutionResult.From(new TextAccess(file));
		BinaryCompound compound = result.Result<BinaryCompound>();
		string space = result.Space;
		Id id = Id.Fallback(space, file.Name);
		ScriptDecoder.SetPrefabProperty(id, compound);
	};

	public static LoadingQueue GetUploadQueueWithProcessors(string space, bool isRoot = false)
	{
		LoadingQueue loader = new LoadingQueue(space);
		if (isRoot)
		{
			loader.BeginTask += BlockItemBuffer.Begin;
			loader.EndTask += BlockItemBuffer.End;
		}
		loader.Processors["lang"] = LangProcessor;
		loader.Processors["image"] = TextureProcessor;
		loader.Processors["soundtrack"] = MusicSoundProcessor;
		loader.Processors["sound"] = MusicSoundProcessor;
		loader.Processors["shader"] = ShaderProcessor;

		loader.Processors["recipe"] = RecipeProcessor;
		loader.Processors["group"] = GroupProcessor;
		loader.Processors["model/binding_block.lab"] = BlockModelProcessor;
		loader.Processors["model/binding_wall.lab"] = WallModelProcessor;
		loader.Processors["property/block"] = BlockPropertyProcessor;
		loader.Processors["property/wall"] = WallPropertyProcessor;
		loader.Processors["property/item"] = ItemPropertyProcessor;
		loader.Processors["property/prefab"] = PrefabPropertyProcessor;
		return loader;
	}

	public static void LoadContents(LoadingQueue loader, PathHandle path)
	{
		loader.Filebase = path;
		loader.Scan(path.Find("lang"), false);
		loader.Scan(path.Find("image"), false);
		loader.Scan(path.Find("soundtrack"), false);
		loader.Scan(path.Find("sound"), false);
		loader.Scan(path.Find("shader"), true);
		loader.Scan(path.Find("model"), false);

		loader.Scan(path.Find("group"), false);
		loader.Scan(path.Find("recipe"), false);
		loader.Scan(path.Find("property"), false);
	}

	public static void LoadFontMaps()
	{
		PathHandle fh1 = Bootstrap.ContentPath.Find(Options.FontLoc);
		PathHandle fh2 = Bootstrap.ContentPath.Find(Options.FontLatinLoc);

		Font fonta = Surface.Current.NewFont(fh1, 128, Options.FontSize);
		Font fontc = Surface.Current.NewFont(fh2, 128, Options.FontLatinSize);

		Bootstrap.FontOverall = new FontCombined(ch =>
		{
			if (fontc.IsCharSupported(ch))
				return fontc;
			return fonta;
		}, fonta);

		Graphics.Global.Font = Bootstrap.FontOverall;
	}

}
