using System;
using Godot;

namespace  Spaceshooter.Scripts.Behaviors;

public sealed partial class RotationController : ParentBehavior
{
    [Export] private float _rotationSpeed = 20F;
    

    public override void _Process(double delta)
    {
        var degreesPerSecond = _rotationSpeed * delta;
        var radiansPerSecond = (float)Mathf.DegToRad(degreesPerSecond);
        Parent?.Rotate(radiansPerSecond);
    }
}
