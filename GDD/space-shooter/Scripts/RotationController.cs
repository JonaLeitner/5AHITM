using System;
using Godot;

namespace  Spaceshooter.Scripts.Behaviors;

public sealed partial class RotationController : Node2D
{
    [Export] private float _rotationSpeed = 20F;
    private Node2D? _parent;

    public override void _Ready()
    {
        _parent = GetParent<Node2D>() ?? throw new InvalidOperationException("No parent found to rotate");
    }

    public override void _Process(double delta)
    {
        var degreesPerSecond = _rotationSpeed * delta;
        var radiansPerSecond = (float)Mathf.DegToRad(degreesPerSecond);
        _parent?.Rotate(radiansPerSecond);
    }
}

