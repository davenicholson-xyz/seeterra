// Chunk.cs
using Godot;
using System.Collections.Generic;

public partial class Chunk : Node3D
{
    private Vector3I     _chunkCoord;
    private ChunkManager _manager;
    private MeshInstance3D _meshInstance;

    // Called by ChunkManager before _Ready fires
    public void Initialise(Vector3I chunkCoord, ChunkManager manager)
    {
        _chunkCoord = chunkCoord;
        _manager    = manager;
    }

    public override void _Ready()
    {
        _meshInstance = new MeshInstance3D();
        AddChild(_meshInstance);
        BuildMesh();
    }

    private Vector3I ToWorld(int x, int y, int z)
    {
        return new Vector3I(
            _chunkCoord.X * ChunkManager.ChunkSize + x,
            _chunkCoord.Y * ChunkManager.ChunkSize + y,
            _chunkCoord.Z * ChunkManager.ChunkSize + z
        );
    }

    private bool IsSolid(int x, int y, int z)
    {
        return _manager.IsSolid(ToWorld(x, y, z));
    }

    private void BuildMesh()
    {
        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs     = new List<Vector2>();
        var indices = new List<int>();

        for (int x = 0; x < ChunkManager.ChunkSize; x++)
        for (int y = 0; y < ChunkManager.WorldHeight + 8; y++)
        for (int z = 0; z < ChunkManager.ChunkSize; z++)
        {
            if (!IsSolid(x, y, z)) continue;

            var block = _manager.GetBlock(ToWorld(x, y, z));

            if (!IsSolid(x, y, z + 1)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Front, block.BlockType);
            if (!IsSolid(x, y, z - 1)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Back, block.BlockType);
            if (!IsSolid(x - 1, y, z)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Left, block.BlockType);
            if (!IsSolid(x + 1, y, z)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Right, block.BlockType);
            if (!IsSolid(x, y + 1, z)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Top, block.BlockType);
            if (!IsSolid(x, y - 1, z)) AddFace(verts, normals, uvs, indices, new Vector3(x,y,z), Face.Bottom, block.BlockType);
        }

        if (verts.Count == 0) return;

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV]  = uvs.ToArray();
        arrays[(int)Mesh.ArrayType.Index]  = indices.ToArray();

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        _meshInstance.Mesh = mesh;

        var texture = ResourceLoader.Load<Texture2D>("res://test_atlas.png");
        var mat = new StandardMaterial3D();
        mat.AlbedoTexture = texture;
        mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;
        _meshInstance.MaterialOverride = mat;

        AddCollision(mesh);
    }

    private (float uMin, float uMax) GetTileUVs(int tileIndex)
    {
        return (tileIndex / 6f, (tileIndex + 1) / 6f);
    }

    private int GetTileIndex(Face face) => face switch
    {
        Face.Front  => 0,
        Face.Back   => 1,
        Face.Left   => 2,
        Face.Right  => 3,
        Face.Top    => 4,
        Face.Bottom => 5,
        _           => 0
    };

    private void AddFace(
        List<Vector3> verts, List<Vector3> normals,
        List<Vector2> uvs,   List<int> indices,
        Vector3 pos, Face face, int blockType)
    {
        int i = verts.Count;

        Vector3 n, v0, v1, v2, v3;

        switch (face)
        {
            case Face.Front:
                n=new(0,0,1);
                v0=new(-0.5f,-0.5f, 0.5f); v1=new( 0.5f,-0.5f, 0.5f);
                v2=new( 0.5f, 0.5f, 0.5f); v3=new(-0.5f, 0.5f, 0.5f);
                break;
            case Face.Back:
                n=new(0,0,-1);
                v0=new( 0.5f,-0.5f,-0.5f); v1=new(-0.5f,-0.5f,-0.5f);
                v2=new(-0.5f, 0.5f,-0.5f); v3=new( 0.5f, 0.5f,-0.5f);
                break;
            case Face.Left:
                n=new(-1,0,0);
                v0=new(-0.5f,-0.5f,-0.5f); v1=new(-0.5f,-0.5f, 0.5f);
                v2=new(-0.5f, 0.5f, 0.5f); v3=new(-0.5f, 0.5f,-0.5f);
                break;
            case Face.Right:
                n=new(1,0,0);
                v0=new( 0.5f,-0.5f, 0.5f); v1=new( 0.5f,-0.5f,-0.5f);
                v2=new( 0.5f, 0.5f,-0.5f); v3=new( 0.5f, 0.5f, 0.5f);
                break;
            case Face.Top:
                n=new(0,1,0);
                v0=new(-0.5f, 0.5f, 0.5f); v1=new( 0.5f, 0.5f, 0.5f);
                v2=new( 0.5f, 0.5f,-0.5f); v3=new(-0.5f, 0.5f,-0.5f);
                break;
            default: // Bottom
                n  = new(0, -1, 0);
                v0 = new(-0.5f, -0.5f, -0.5f);
                v1 = new( 0.5f, -0.5f, -0.5f);
                v2 = new( 0.5f, -0.5f,  0.5f);
                v3 = new(-0.5f, -0.5f,  0.5f);
                break;
        }

        verts.Add(pos+v0); verts.Add(pos+v1);
        verts.Add(pos+v2); verts.Add(pos+v3);

        normals.Add(n); normals.Add(n); normals.Add(n); normals.Add(n);

        var def      = BlockRegistry.Get(blockType);
        int tileIndex = def.GetTileIndex(face);
        float uMin   = BlockRegistry.UMin(tileIndex);
        float uMax   = BlockRegistry.UMax(tileIndex);

        uvs.Add(new(uMin, 1)); uvs.Add(new(uMax, 1));
        uvs.Add(new(uMax, 0)); uvs.Add(new(uMin, 0));
        
        // var (uMin, uMax) = GetTileUVs(GetTileIndex(face));
        // uvs.Add(new(uMin,1)); uvs.Add(new(uMax,1));
        // uvs.Add(new(uMax,0)); uvs.Add(new(uMin,0));

        indices.Add(i);   indices.Add(i+2); indices.Add(i+1);
        indices.Add(i);   indices.Add(i+3); indices.Add(i+2);
    }

    private void AddCollision(ArrayMesh mesh)
    {
        // Remove old collision if rebuilding
        var existing = GetNodeOrNull<StaticBody3D>("StaticBody3D");
        existing?.QueueFree();

        var body  = new StaticBody3D { Name = "StaticBody3D" };
        var shape = new CollisionShape3D();
        shape.Shape = mesh.CreateTrimeshShape();
        body.AddChild(shape);
        AddChild(body);
    }
}