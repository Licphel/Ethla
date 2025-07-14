using System;
using Ethla.Api;
using Ethla.Common.Voxel;
using Ethla.World.Voxel;
using Spectrum.Core.Manage;

namespace Ethla.Common;

public class Walls
{

    public static readonly IdMap<Wall> Registry = ModRegistry.Walls;

    public static Wall Empty = Registry.RegisterDefaultValue("ethla:empty", new Wall()).DeriveItem();
    public static Wall Dirt = Registry.Register("ethla:dirt", new Wall()).DeriveItem();
    public static Wall Rock = Registry.Register("ethla:rock", new Wall()).DeriveItem();
    public static Wall Sand = Registry.Register("ethla:sand", new Wall()).DeriveItem();
    public static Wall Sandstone = Registry.Register("ethla:sandstone", new Wall()).DeriveItem();

}
