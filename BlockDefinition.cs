// BlockDefinition.cs
public struct BlockDefinition
{
    public string Name;
    public int TileFront;
    public int TileBack;
    public int TileLeft;
    public int TileRight;
    public int TileTop;
    public int TileBottom;

    public int GetTileIndex(Face face)
    {
        return face switch
        {
            Face.Front  => TileFront,
            Face.Back   => TileBack,
            Face.Left   => TileLeft,
            Face.Right  => TileRight,
            Face.Top    => TileTop,
            Face.Bottom => TileBottom,
            _           => TileFront
        };
    }


// All 6 faces the same
public static BlockDefinition Uniform(string name, int tile) => new BlockDefinition
{
    Name       = name,
    TileFront  = tile, TileBack  = tile,
    TileLeft   = tile, TileRight = tile,
    TileTop    = tile, TileBottom = tile,
};

// Unique top/bottom, same on all sides
public static BlockDefinition Capped(string name, int top, int side, int bottom) => new BlockDefinition
{
    Name       = name,
    TileFront  = side, TileBack  = side,
    TileLeft   = side, TileRight = side,
    TileTop    = top,  TileBottom = bottom,
};

// Full control
public static BlockDefinition Custom(string name,
    int front, int back, int left, int right, int top, int bottom) => new BlockDefinition
{
    Name       = name,
    TileFront  = front, TileBack  = back,
    TileLeft   = left,  TileRight = right,
    TileTop    = top,   TileBottom = bottom,
};

}
