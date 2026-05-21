using Godot;
using System.Collections.Generic;

public partial class ChunkManager : Node3D
{
    public const int ChunkSize = 16;
    public const int WorldHeight = 3;
    public const int WorldSize = 3;

    private Dictionary<Vector3I, BlockData>   _blocks = new();
    private Dictionary<Vector3I, Chunk> _chunks = new();

    public override void _Ready()
    {
        GenerateBlockData();
        CreateChunks();
    }

    private void GenerateBlockData()
    {
        var rng = new System.Random();
        int worldWidth = ChunkSize * WorldSize;

        for (int x = 0; x < worldWidth; x++)
		for (int y = 0; y < WorldHeight; y++)
        for (int z = 0; z < worldWidth; z++)
        {
            if (rng.NextDouble() < 0.3) continue;
            _blocks[new Vector3I(x, y, z)] = BlockData.Solid(rng.Next(0,3));
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
}