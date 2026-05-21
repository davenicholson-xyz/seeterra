// BlockData.cs
public struct BlockData
{
    public bool IsAir;
    public int  BlockType;
    public bool IsSolid => !IsAir;

    public static BlockData Air => new BlockData { IsAir = true, BlockType = 0 };

    public static BlockData Solid(int blockType) => new BlockData
    {
        IsAir     = false,
        BlockType = blockType
    };
}