// BlockRegistry.cs
public static class BlockRegistry
{
    public const int TileCount = 3; // total tiles in atlas — update as you add more

    private static readonly BlockDefinition[] _definitions = new BlockDefinition[]
    {
        BlockDefinition.Capped("Grass", top: 0, side: 0, bottom: 1),

        // Index 1 — Dirt: same on all faces
        BlockDefinition.Uniform("Dirt", tile: 1),

        // Index 2 — Stone: same on all faces
        BlockDefinition.Uniform("Stone", tile: 2),

        // Index 3 — A weird block with all different faces
        BlockDefinition.Custom("Weird", front: 0, back: 1, left: 2, right: 0, top: 1, bottom: 2),
    };

    public static BlockDefinition Get(int blockType)
    {
        // Safety check — unknown type falls back to stone
        if (blockType < 0 || blockType >= _definitions.Length)
            return _definitions[2];

        return _definitions[blockType];
    }

    public static float UMin(int tileIndex) => tileIndex / (float)TileCount;
    public static float UMax(int tileIndex) => (tileIndex + 1) / (float)TileCount;
}