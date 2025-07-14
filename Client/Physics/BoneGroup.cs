using Spectrum.Graphic.Images;

namespace Ethla.Client.Physics;

public class BoneGroup
{

	public List<Bone> Begin = new List<Bone>();
	public Icon Body;
	public List<Bone> End = new List<Bone>();

	public BoneGroup SetBody(Icon icon)
	{
		Body = icon;
		return this;
	}

	public BoneGroup AddToFront(Bone part)
	{
		End.Add(part);
		return this;
	}

	public BoneGroup AddToBack(Bone part)
	{
		Begin.Add(part);
		return this;
	}

}
