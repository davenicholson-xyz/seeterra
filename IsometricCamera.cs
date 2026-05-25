using Godot;

public partial class IsometricCamera : Camera3D
{
    [Export] public float MoveSpeed = 20f;
    [Export] public float ZoomMin   = 5f;
    [Export] public float ZoomMax   = 80f;
    [Export] public float ZoomStep  = 3f;

    public override void _Ready()
    {
        // Lock to isometric angle
        Projection = ProjectionType.Orthogonal;
        Rotation   = new Vector3(Mathf.DegToRad(-35.264f), Mathf.DegToRad(45f), 0f);
        Size       = 20f;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse && mouse.Pressed)
        {
            if (mouse.ButtonIndex == MouseButton.WheelUp)
                Size = Mathf.Clamp(Size - ZoomStep, ZoomMin, ZoomMax);
            if (mouse.ButtonIndex == MouseButton.WheelDown)
                Size = Mathf.Clamp(Size + ZoomStep, ZoomMin, ZoomMax);
        }
    }

    public override void _Process(double delta)
    {
        HandleMovement((float)delta);
    }

    private void HandleMovement(float delta)
    {
        // Move along world X and Z axes so direction
        // feels correct relative to the isometric view
        Vector3 dir = Vector3.Zero;

        if (Input.IsKeyPressed(Key.W)) dir += new Vector3(-1, 0, -1);
        if (Input.IsKeyPressed(Key.S)) dir += new Vector3( 1, 0,  1);
        if (Input.IsKeyPressed(Key.A)) dir += new Vector3(-1, 0,  1);
        if (Input.IsKeyPressed(Key.D)) dir += new Vector3( 1, 0, -1);

        if (Input.IsKeyPressed(Key.Q)) Size = Mathf.Clamp(Size - (ZoomStep / 2), ZoomMin, ZoomMax);
        if (Input.IsKeyPressed(Key.E)) Size = Mathf.Clamp(Size + (ZoomStep / 2), ZoomMin, ZoomMax);


        if (dir != Vector3.Zero)
            Position += dir.Normalized() * MoveSpeed * delta;
    }
}