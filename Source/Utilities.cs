using System.Net;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Powderkeg;

public enum Material
{
    Air,
    Sand,
    Water
}

public enum Phase
{
    Empty,
    Solid,
    Liquid,
    Gas,
    Plasma
}

public readonly struct MaterialProperties
{
    public Color Color {get; init; }
    public Phase Phase {get; init; }
    public int Density {get; init; }

}

public static class MaterialRegistry
{
    private static readonly MaterialProperties[] Properties = new MaterialProperties[]
    {
        new MaterialProperties {Color = Color.Black, Phase = Phase.Empty, Density = 0},
        new MaterialProperties {Color = Color.SandyBrown, Phase = Phase.Solid, Density = 50},
        new MaterialProperties {Color = Color.Blue, Phase = Phase.Liquid, Density = 10}
    };

    public static MaterialProperties Get(Material material)
    {
        return Properties[(int)material];
    }
}