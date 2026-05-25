using Godot;

public partial class IsometricCamera : Camera3D
{
    [Export] public float MoveSpeed   = 20f;
    [Export] public float ZoomMin     = 5f;
    [Export] public float ZoomMax     = 80f;
    [Export] public float ZoomStep    = 3f;
    [Export] public float RotateSpeed = 5f;

    private static readonly float[] _snapAngles = { 45f, 135f, 225f, 315f };

    private int     _snapIndex  = 0;
    private float   _currentYaw = 45f;
    private float   _targetYaw  = 45f;
    private bool    _rotating   = false;
    private Vector3 _rotationFocus;

    private const float Pitch = -35.264f;

    public override void _Ready()
    {
        Projection = ProjectionType.Orthogonal;
        Rotation   = new Vector3(Mathf.DegToRad(Pitch), Mathf.DegToRad(45f), 0f);
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

        if (@event is InputEventKey key && key.Pressed && !key.Echo && !_rotating)
        {
            if (key.Keycode == Key.Z) StartRotation(-1);
            if (key.Keycode == Key.X) StartRotation( 1);
        }
    }

    private void StartRotation(int direction)
    {
        _rotationFocus = ComputeScreenCentreFocus();
        _snapIndex     = Mathf.PosMod(_snapIndex + direction, 4);
        _targetYaw     = _snapAngles[_snapIndex];
        _rotating      = true;
    }

    // Raycast through the centre of the viewport to find the actual terrain point.
    private Vector3 ComputeScreenCentreFocus()
    {
        var centre = GetViewport().GetVisibleRect().Size / 2f;
        var from   = ProjectRayOrigin(centre);
        var to     = from + ProjectRayNormal(centre) * 2000f;

        var query  = PhysicsRayQueryParameters3D.Create(from, to);
        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

        if (result.Count > 0)
            return result["position"].AsVector3();

        // Fallback: intersect with Y=0 if no terrain under the centre
        var dir = ProjectRayNormal(centre);
        float t = Mathf.Abs(dir.Y) > 0.001f ? -from.Y / dir.Y : 500f;
        return from + dir * t;
    }

    public override void _Process(double delta)
    {
        HandleMovement((float)delta);
        HandleRotation((float)delta);
    }

    private void HandleRotation(float delta)
    {
        if (!_rotating) return;

        float diff = Mathf.AngleDifference(
            Mathf.DegToRad(_currentYaw),
            Mathf.DegToRad(_targetYaw));

        if (Mathf.Abs(diff) < 0.01f)
        {
            _currentYaw = _targetYaw;
            _rotating   = false;
        }
        else
        {
            _currentYaw += Mathf.RadToDeg(diff) * Mathf.Min(RotateSpeed * delta, 1f);
        }

        float yaw   = Mathf.DegToRad(_currentYaw);
        float pitch = Mathf.DegToRad(Pitch);

        // Horizontal orbit radius derived from the vertical distance to the focus point
        float vertDist = Position.Y - _rotationFocus.Y;
        float r        = vertDist * Mathf.Cos(Mathf.Abs(pitch)) / Mathf.Sin(Mathf.Abs(pitch));

        Position = new Vector3(
            _rotationFocus.X + r * Mathf.Sin(yaw),
            Position.Y,
            _rotationFocus.Z + r * Mathf.Cos(yaw));

        Rotation = new Vector3(pitch, yaw, 0f);
    }

    private void HandleMovement(float delta)
    {
        Vector3 dir = Vector3.Zero;

        float yaw     = Mathf.DegToRad(_currentYaw);
        var   forward = new Vector3(-Mathf.Sin(yaw), 0, -Mathf.Cos(yaw));
        var   right   = new Vector3( Mathf.Cos(yaw), 0, -Mathf.Sin(yaw));

        if (Input.IsKeyPressed(Key.W)) dir += forward;
        if (Input.IsKeyPressed(Key.S)) dir -= forward;
        if (Input.IsKeyPressed(Key.A)) dir -= right;
        if (Input.IsKeyPressed(Key.D)) dir += right;

        if (Input.IsKeyPressed(Key.Q))
            Size = Mathf.Clamp(Size - ZoomStep / 2, ZoomMin, ZoomMax);
        if (Input.IsKeyPressed(Key.E))
            Size = Mathf.Clamp(Size + ZoomStep / 2, ZoomMin, ZoomMax);

        if (dir != Vector3.Zero)
            Position += dir.Normalized() * MoveSpeed * delta;
    }
}
