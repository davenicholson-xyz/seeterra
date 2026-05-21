// BlockData.cs
public struct BlockData
{
    public bool IsAir;
    public int  BlockType; // 0=air, 1=grass, 2=dirt, 3=stone etc
    public bool IsSolid => !IsAir;

    // Convenience factory methods so callsites are readable
    public static BlockData Air => new BlockData { IsAir = true, BlockType = 0 };

    public static BlockData Solid(int blockType) => new BlockData
    {
        IsAir     = false,
        BlockType = blockType
    };
}