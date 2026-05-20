using Godot;

public partial class VoxelCube : Node3D
{
    public override void _Ready()
    {
        var meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        meshInstance.Mesh = BuildCubeMesh();

        var texture = ResourceLoader.Load<Texture2D>("res://grass.jpg");
        var mat = new StandardMaterial3D();
        mat.AlbedoTexture = texture;
        meshInstance.MaterialOverride = mat;
    }

    private ArrayMesh BuildCubeMesh()
    {
        var vertices = new Vector3[]
        {
            // Front
            new(-0.5f, -0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(-0.5f, 0.5f, 0.5f),
            // Back
            new( 0.5f, -0.5f, -0.5f),  
            new(-0.5f, -0.5f, -0.5f),  
            new(-0.5f,  0.5f, -0.5f),  
            new( 0.5f,  0.5f, -0.5f),  
            // Left
            new(-0.5f, -0.5f, -0.5f),  
            new(-0.5f, -0.5f,  0.5f),  
            new(-0.5f,  0.5f,  0.5f),  
            new(-0.5f,  0.5f, -0.5f),  
            // Right
            new( 0.5f, -0.5f,  0.5f),  
            new( 0.5f, -0.5f, -0.5f),  
            new( 0.5f,  0.5f, -0.5f),  
            new( 0.5f,  0.5f,  0.5f),  
            // Top 
            new(-0.5f,  0.5f,  0.5f),  
            new( 0.5f,  0.5f,  0.5f),  
            new( 0.5f,  0.5f, -0.5f),  
            new(-0.5f,  0.5f, -0.5f),  
            // Bottom
            new( 0.5f, -0.5f, -0.5f),  
            new(-0.5f, -0.5f, -0.5f),  
            new(-0.5f, -0.5f,  0.5f),  
            new( 0.5f, -0.5f,  0.5f),  
            
        };

        var normals = new Vector3[]
        {
            new( 0, 0, 1), new( 0, 0, 1), new( 0, 0, 1), new( 0, 0, 1),  // Front
            new( 0, 0,-1), new( 0, 0,-1), new( 0, 0,-1), new( 0, 0,-1),  // Back
            new(-1, 0, 0), new(-1, 0, 0), new(-1, 0, 0), new(-1, 0, 0),  // Left
            new( 1, 0, 0), new( 1, 0, 0), new( 1, 0, 0), new( 1, 0, 0),  // Right
            new( 0, 1, 0), new( 0, 1, 0), new( 0, 1, 0), new( 0, 1, 0),  // Top
            new( 0,-1, 0), new( 0,-1, 0), new( 0,-1, 0), new( 0,-1, 0),  // Bottom
        };
        
        var indices = new int[]
        {
            0,  2,  1,   0,  3,  2,  // Front
            4,  6,  5,   4,  7,  6,  // Back
            8, 10,  9,   8, 11, 10,  // Left
            12, 14, 13,  12, 15, 14,  // Right
            16, 18, 17,  16, 19, 18,  // Top
            20, 22, 21,  20, 23, 22,  // Bottom
        };

        var uvs = new Vector2[]
        {
            new(0,1), new(1,1), new(1,0), new(0,0),  // Front
            new(0,1), new(1,1), new(1,0), new(0,0),  // Back
            new(0,1), new(1,1), new(1,0), new(0,0),  // Left
            new(0,1), new(1,1), new(1,0), new(0,0),  // Right
            new(0,1), new(1,1), new(1,0), new(0,0),  // Top
            new(0,1), new(1,1), new(1,0), new(0,0),  // Bottom
        };

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.Normal] = normals;
        arrays[(int)Mesh.ArrayType.TexUV] = uvs;
        arrays[(int)Mesh.ArrayType.Index] = indices;

        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh;
    }
}