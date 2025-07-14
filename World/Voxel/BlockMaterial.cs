using Ethla.Client.Audio;
using Spectrum.Core.Manage;

namespace Ethla.World.Voxel;

public class BlockMaterial : IdHolder
{

	public virtual Sound GetNormalSound(string type)
	{
		return new Sound($"{Uid.Space}:sound/block/{Uid.Key}/{type}.wav");
	}

}
