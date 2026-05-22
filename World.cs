using Godot;

public partial class World : Node3D
{
    private Camera3D     _camera;
    private Node3D       _highlight;
    private ChunkManager _chunkManager;

    private bool     _hasHover;
    private Vector3I _hoveredVoxel;
    private Vector3  _hoveredNormal;

    public override void _Ready()
    {
        _camera       = GetNode<Camera3D>("Camera3D");
        _chunkManager = GetNode<ChunkManager>("ChunkManager");

        var scene = ResourceLoader.Load<PackedScene>("res://highlight.tscn");
        _highlight = scene.Instantiate<Node3D>();
        _highlight.Visible = false;
        AddChild(_highlight);
    }

    public override void _Process(double delta)
    {
        var mousePos = GetViewport().GetMousePosition();
        var from     = _camera.ProjectRayOrigin(mousePos);
        var to       = from + _camera.ProjectRayNormal(mousePos) * 1000f;

        var query  = PhysicsRayQueryParameters3D.Create(from, to);
        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            var hit    = result["position"].AsVector3();
            var normal = result["normal"].AsVector3();
            var inner  = hit - normal * 0.5f;
            var coord  = new Vector3I(
                Mathf.RoundToInt(inner.X),
                Mathf.RoundToInt(inner.Y),
                Mathf.RoundToInt(inner.Z));

            _hoveredVoxel  = coord;
            _hoveredNormal = normal;
            _hasHover      = true;

            _highlight.Position = new Vector3(coord.X, coord.Y, coord.Z);
            _highlight.Visible  = true;
        }
        else
        {
            _hasHover          = false;
            _highlight.Visible = false;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_hasHover) return;
        if (@event is not InputEventMouseButton mb || !mb.Pressed) return;

        if (mb.ButtonIndex == MouseButton.Right)
        {
            _chunkManager.SetBlock(_hoveredVoxel, BlockData.Air);
        }
        else if (mb.ButtonIndex == MouseButton.Left)
        {
            var placePos = _hoveredVoxel + new Vector3I(
                Mathf.RoundToInt(_hoveredNormal.X),
                Mathf.RoundToInt(_hoveredNormal.Y),
                Mathf.RoundToInt(_hoveredNormal.Z));
            _chunkManager.SetBlock(placePos, BlockData.Solid(0));
        }
    }
}
