namespace Ethla.World.Voxel;

public ref struct LiquidStack
{

	public int Amount;
	public Liquid Liquid;
	public float Percent => (float)Amount / Liquid.MaxAmount;

	public LiquidStack()
	{
		Amount = 0;
		Liquid = Liquid.Empty;
	}

	public LiquidStack(Liquid liquid, int amount)
	{
		Amount = Math.Clamp(amount, 0, Liquid.MaxAmount);
		Liquid = amount == 0 ? Liquid.Empty : liquid;
	}

}
