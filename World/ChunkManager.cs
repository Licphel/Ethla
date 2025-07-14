using Spectrum.IO.Pathfind;
using Spectrum.Maths;

namespace Ethla.World;

public class ChunkManager
{

	private readonly Dictionary<int, ChunkBuffer> buffers = new Dictionary<int, ChunkBuffer>();
	private readonly PathHandle file0;
	private readonly List<int> forRemove = new List<int>();

	public PathHandle LevelSave;

	public ChunkManager(LevelType type)
	{
		file0 = Bootstrap.SavePath.Find($"{type.Id.Key}/chunks");
		LevelSave = Bootstrap.SavePath.Find($"{type.Id.Key}/level_data.bin");
		file0.Mkdirs();
	}

	public void CheckOldBuffers(Level level)
	{
		foreach (KeyValuePair<int, ChunkBuffer> kv in buffers)
			if (!kv.Value.IsBufferAvailable(level))
				forRemove.Add(kv.Key);
		foreach (int i in forRemove) buffers.Remove(i);
		forRemove.Clear();
	}

	public ChunkBuffer Buffer(Chunk chunk)
	{
		int coord = Mathf.FastFloor((float)chunk.Coord / ChunkBuffer.Size);
		if (buffers.ContainsKey(coord)) return buffers[coord];
		buffers[coord] = new ChunkBuffer(file0, coord);
		return buffers[coord];
	}

	public bool IsChunkArchived(Chunk chunk)
	{
		return Buffer(chunk).IsChunkArchived(chunk);
	}

	public void Read(Chunk chunk)
	{
		Buffer(chunk).TryRead(chunk);
	}

	public void WriteToBuffer(Chunk chunk, bool removal = false)
	{
		Buffer(chunk).WriteToBuffer(chunk, removal);
	}

	public void WriteToDisk()
	{
		foreach (ChunkBuffer buffer in buffers.Values) buffer.TryWrite();
	}

}
