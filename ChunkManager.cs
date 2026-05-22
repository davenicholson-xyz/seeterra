using Godot;
using System.Collections.Generic;

public partial class ChunkManager : Node3D
{
    public const int ChunkSize = 16;
    public const int WorldHeight = 16;
    public const int WorldSize = 9;

    private Dictionary<Vector3I, BlockData>   _blocks = new();
    private Dictionary<Vector3I, Chunk> _chunks = new();
    private FastNoiseLite
     _noise;

    public override void _Ready()
    {
        SetupNoise();
        GenerateBlockData();
        CreateChunks();
        CreateChunkDebugLines();
    }

    private void SetupNoise()
    {
        _noise = new FastNoiseLite();
        _noise.Seed = 1337;
        _noise.Frequency = 0.005f;
        _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
    } 

    private void GenerateBlockData()
    {
        // var rng = new System.Random();
        int worldWidth = ChunkSize * WorldSize;

        for (int x = 0; x < worldWidth; x++)
        for (int z = 0; z < worldWidth; z++)
        {
            float raw    = _noise.GetNoise2D(x, z);         // -1 to 1
            float norm   = (raw + 1f) / 2f;                 // 0 to 1
            int   height = (int)(norm * WorldHeight) + 4;   // 4 to 36

            for (int y = 0; y <= height; y++)
            {
                int blockType;

                if (y == height)
                    blockType = 0;
                else if (y >= height - 3)
                    blockType = 1;
                else
                    blockType = 2;

                _blocks[new Vector3I(x, y, z)] = BlockData.Solid(blockType);
            }
        }
    }

    public BlockData GetBlock(Vector3I worldPos) 
    {
        if (_blocks.TryGetValue(worldPos, out BlockData data))
            return data;

        return BlockData.Air;
    }

    public bool IsSolid(Vector3I worldPos) => GetBlock(worldPos).IsSolid;


    private void CreateChunks()
    {
        for (int cx = 0; cx < WorldSize; cx++)
        for (int cz = 0; cz < WorldSize; cz++)
        {
            var chunkCoord = new Vector3I(cx, 0, cz);

            var chunk = new Chunk();
            chunk.Initialise(chunkCoord, this);
            chunk.Position = new Vector3( cx * ChunkSize, 0, cz * ChunkSize );
            AddChild(chunk);
            _chunks[chunkCoord] = chunk;
        }
    }

    public void SetBlock(Vector3I worldPos, BlockData data)
    {
        if (data.IsSolid)
            _blocks[worldPos] = data;
        else
            _blocks.Remove(worldPos);

        RebuildChunkAt(worldPos);

        // Rebuild the neighbour chunk too if the block sits on a border
        int lx = ((worldPos.X % ChunkSize) + ChunkSize) % ChunkSize;
        int lz = ((worldPos.Z % ChunkSize) + ChunkSize) % ChunkSize;
        if (lx == 0)             RebuildChunkAt(worldPos + new Vector3I(-1, 0, 0));
        if (lx == ChunkSize - 1) RebuildChunkAt(worldPos + new Vector3I( 1, 0, 0));
        if (lz == 0)             RebuildChunkAt(worldPos + new Vector3I( 0, 0, -1));
        if (lz == ChunkSize - 1) RebuildChunkAt(worldPos + new Vector3I( 0, 0,  1));
    }

    private void RebuildChunkAt(Vector3I worldPos)
    {
        var chunkCoord = new Vector3I(
            Mathf.FloorToInt((float)worldPos.X / ChunkSize),
            0,
            Mathf.FloorToInt((float)worldPos.Z / ChunkSize));

        if (_chunks.TryGetValue(chunkCoord, out Chunk chunk))
            chunk.Rebuild();
    }

    [Export] public bool DebugChunkBorders = false;

private void CreateChunkDebugLines()
{
    if (!DebugChunkBorders) return;

    int worldWidth = ChunkSize * WorldSize;
    int lineHeight = WorldHeight + 10; // a bit taller than max terrain

    // Draw a vertical line at every chunk corner
    for (int cx = 0; cx <= WorldSize; cx++)
    for (int cz = 0; cz <= WorldSize; cz++)
    {
        float wx = cx * ChunkSize;
        float wz = cz * ChunkSize;

        var verts = new Vector3[]
        {
            new(wx, 0,          wz),
            new(wx, lineHeight, wz),
        };

        var colors = new Color[]
        {
            new(1, 0, 1, 1),  // magenta
            new(1, 0, 1, 1),
        };

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = verts;
        arrays[(int)Mesh.ArrayType.Color]  = colors;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

        var mat = new StandardMaterial3D();
        mat.ShadingMode        = BaseMaterial3D.ShadingModeEnum.Unshaded;
        mat.VertexColorUseAsAlbedo = true;

        var mi = new MeshInstance3D();
        mi.Mesh             = mesh;
        mi.MaterialOverride = mat;
        mi.CastShadow       = GeometryInstance3D.ShadowCastingSetting.Off;
        AddChild(mi);
    }
}

}