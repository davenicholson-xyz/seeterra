using Godot;

public partial class FreeCam : Camera3D
{
    [Export] public float MoveSpeed = 10f;
    [Export] public float SpeedMultiplier = 3f;
    [Export] public float MouseSensitivity = 0.003f;

    private Vector2 _mouseDelta;
    private bool _captured = false;

    public override void _Ready()
    {
        CaptureMouse();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && _captured)
        {
            _mouseDelta = mouseMotion.Relative;
        }

        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
                CaptureMouse();
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
                ReleaseMouse();
        }
    }

    public override void _Process(double delta)
    {
        if (!_captured) return;

        HandleRotation();
        HandleMovement((float)delta);
    }

    private void HandleRotation()
    {
        if (_mouseDelta == Vector2.Zero) return;

        RotateY(-_mouseDelta.X * MouseSensitivity);
        RotateObjectLocal(Vector3.Right, _mouseDelta.Y * MouseSensitivity);

        // Clamp pitch
        Vector3 rot = Rotation;
        rot.X = Mathf.Clamp(rot.X, Mathf.DegToRad(-89f), Mathf.DegToRad(89f));
        Rotation = rot;

        _mouseDelta = Vector2.Zero;
    }

    private void HandleMovement(float delta)
    {
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("ui_up")    || Input.IsKeyPressed(Key.W)) direction -= Transform.Basis.Z;
        if (Input.IsActionPressed("ui_down")  || Input.IsKeyPressed(Key.S)) direction += Transform.Basis.Z;
        if (Input.IsActionPressed("ui_left")  || Input.IsKeyPressed(Key.A)) direction -= Transform.Basis.X;
        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D)) direction += Transform.Basis.X;
        if (Input.IsKeyPressed(Key.E)) direction += Vector3.Up;
        if (Input.IsKeyPressed(Key.Q)) direction += Vector3.Down;

        float speed = Input.IsKeyPressed(Key.Shift) ? MoveSpeed * SpeedMultiplier : MoveSpeed;

        if (direction != Vector3.Zero)
            Position += direction.Normalized() * speed * delta;
    }

    private void CaptureMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _captured = true;
    }

    private void ReleaseMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        _captured = false;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusIn)
            CaptureMouse();
    }
}