using System.Runtime.CompilerServices;
using Ethla.World.Voxel;
using Spectrum.Graphic;

namespace Ethla.World.Lighting;

public unsafe class LightWare
{

	public static LightWare Empty = new LightWare(null);

	public Chunk Chunk;
	public float[] Light = new float[6];//0,1,2 - block 3,4,5 - sky
	public float[] SmoothedLight = new float[24];//0-11 - block 12-23 - sky
	public float[] AoLight = new float[4];
	public float[] RealLight = new float[12];
	public bool IsDark;

	public LightWare(Chunk chunk)
	{
		Chunk = chunk;
	}

	public void Submit(int x, int y, float[] sunlight)
	{
		MakeAoAndAverageLight(x, y, 0, 0, 0);
		MakeAoAndAverageLight(x, y, 1, 0, 0);
		MakeAoAndAverageLight(x, y, 2, 0, 0);
		MakeAoAndAverageLight(x, y, 0, 3, 12);
		MakeAoAndAverageLight(x, y, 1, 3, 12);
		MakeAoAndAverageLight(x, y, 2, 3, 12);

		//Merge real sunlight into skylight gradient.
		for (int i = 0; i < 12; i++)
		{
			SmoothedLight[i + 12] *= sunlight[i % 3];
		}

		IsDark = true;

		//Get the final product.
		for (int i = 0; i < 12; i++)
		{
			float c = RealLight[i] = Math.Max(SmoothedLight[i], SmoothedLight[i + 12]) * LightEngine.Amplifier;
			if (c > LightEngine.DarkLuminance)
				IsDark = false;
		}
	}

	public void GetVec4Out(ref Color v, int i, bool isWall)
	{
		if (Main.GodMode)
		{
			v.R = v.G = v.B = isWall ? 0.75f : 1f;
			v.A = 1;
			return;
		}
		
		v.R = RealLight[0 + i * 3];
		v.G = RealLight[1 + i * 3];
		v.B = RealLight[2 + i * 3];
		v.A = 1;

		if (isWall)
		{
			float k = AoLight[i] * 0.75f;
			v.R *= k;
			v.G *= k;
			v.B *= k;
		}
	}

	public void GetBlockLights(Color[] color, bool isWall)
	{
		GetVec4Out(ref color[0], 0, isWall);
		GetVec4Out(ref color[1], 1, isWall);
		GetVec4Out(ref color[2], 2, isWall);
		GetVec4Out(ref color[3], 3, isWall);
	}

	private void set(int i, float v, int idx, int offset)
	{
		SmoothedLight[i * 3 + idx + offset] = v;
	}

	private void setAo(int i, float v)
	{
		AoLight[i] = v;
	}

	public void MakeAoAndAverageLight(int x, int y, int idx, int offsetLow, int offset)
	{
		LightEngine le = Chunk.Level.LightEngine;

		Chunk c0 = le.GetBufferedChunk(x - 1);
		Chunk c1 = le.GetBufferedChunk(x);
		Chunk c2 = le.GetBufferedChunk(x + 1);

		if (c0 == null || c1 == null || c2 == null) return;

		//calculating once is enough.
		if (offset == 0)
		{
			BlockState b = c1.GetBlock(x, y);

			const float aoS = 0.1f;

			int c = 0;
			if (b.GetShape() == BlockShape.Solid)
			{
				setAo(0, 1 - aoS * 1.5f);
				setAo(1, 1 - aoS * 1.5f);
				setAo(2, 1 - aoS * 1.5f);
				setAo(3, 1 - aoS * 1.5f);
			}
			else
			{
				bool bcc1, bcc3, bcc4, bcc6;
				BlockState b0 = c0.GetBlock(x - 1, y - 1);
				BlockState b1 = c0.GetBlock(x - 1, y);
				BlockState b2 = c0.GetBlock(x - 1, y + 1);

				BlockState b3 = c1.GetBlock(x, y - 1);
				BlockState b4 = c1.GetBlock(x, y + 1);

				BlockState b5 = c2.GetBlock(x + 1, y - 1);
				BlockState b6 = c2.GetBlock(x + 1, y);
				BlockState b7 = c2.GetBlock(x + 1, y + 1);

				if (b0.GetShape() == BlockShape.Solid) c++;
				if (bcc1 = b1.GetShape() == BlockShape.Solid) c++;
				if (bcc3 = b3.GetShape() == BlockShape.Solid) c++;
				setAo(0, 1 - c * aoS);

				c = 0;
				if (bcc1) c++;
				if (b2.GetShape() == BlockShape.Solid) c++;
				if (bcc4 = b4.GetShape() == BlockShape.Solid) c++;
				setAo(1, 1 - c * aoS);

				c = 0;
				if (bcc4) c++;
				if (bcc6 = b6.GetShape() == BlockShape.Solid) c++;
				if (b7.GetShape() == BlockShape.Solid) c++;
				setAo(2, 1 - c * aoS);

				c = 0;
				if (bcc3) c++;
				if (b5.GetShape() == BlockShape.Solid) c++;
				if (bcc6) c++;
				setAo(3, 1 - c * aoS);
			}
		}

		float arr1 = c0.GetSurLightware(x - 1, y).Light[idx + offsetLow];
		float arr2 = c2.GetSurLightware(x + 1, y).Light[idx + offsetLow];
		float arr3 = c1.GetSurLightware(x, y - 1).Light[idx + offsetLow];
		float arr4 = c1.GetSurLightware(x, y + 1).Light[idx + offsetLow];
		float arr5 = c0.GetSurLightware(x - 1, y - 1).Light[idx + offsetLow];
		float arr6 = c2.GetSurLightware(x + 1, y + 1).Light[idx + offsetLow];
		float arr7 = c0.GetSurLightware(x - 1, y + 1).Light[idx + offsetLow];
		float arr8 = c2.GetSurLightware(x + 1, y - 1).Light[idx + offsetLow];
		float l0 = Light[idx + offsetLow];

		set(0, (l0 + arr1 + arr3 + arr5) / 4, idx, offset);
		set(1, (l0 + arr1 + arr4 + arr7) / 4, idx, offset);
		set(2, (l0 + arr2 + arr4 + arr6) / 4, idx, offset);
		set(3, (l0 + arr2 + arr3 + arr8) / 4, idx, offset);
	}

}
