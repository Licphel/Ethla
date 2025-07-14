using Ethla.Common.Iteming;
using Ethla.Common.Voxel;
using Ethla.World.Iteming;
using Spectrum.Graphic.Text;
using Spectrum.Graphic.Ui;
using Spectrum.Maths;

namespace Ethla.Client.Ui;

public class GuiFurnace : GuiSlotLayout
{

	private ImageViewDynamic d1, d2;

	public GuiFurnace(SlotLayout container) : base(container, Images.GuiFurnace, new Vector2(219, 211), Lore.Translate("ethla:block.furnace"))
	{
	}

	public override void InitComponents()
	{
		base.InitComponents();

		Join(d1 = new ImageViewDynamic(Background.Subimage(16, 239, 28, 17), ImageViewStyle.RightShrink));
		Join(d2 = new ImageViewDynamic(Background.Subimage(0, 246, 16, 10), ImageViewStyle.UpShrink));
	}

	public override void RelocateComponents()
	{
		base.RelocateComponents();

		d1.Bound.Set(I + 139, J + 139, 28, 17);
		d2.Bound.Set(I + 48, J + 139, 16, 10);
	}

	public override void Tick(ImVector2 cursor)
	{
		base.Tick(cursor);

		BlockEntityFurnace f = ((SlotLayoutFurnace)Layout).Furnace;
		d1.Progress = Mathf.SafeDiv(f.Cooktime, f.MaxTime);
		d2.Progress = Mathf.SafeDiv(f.Fuel, f.MaxFuel);
	}

}
