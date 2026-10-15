using System;
using Godot;

namespace SpaceShooter.Scripts.Behaviors;

public sealed partial class LinearMovementController : ParentBehavior
{
    [Export]
    private LinearMovementDirection _direction = LinearMovementDirection.Up;

    [Export]
    private float _speed = 80F;

    private bool _isMoving = true;
    

    public override void _PhysicsProcess(double delta)
    {

        if (!_isMoving)
        {
            return;
        }
        
        var direction = _direction switch
        {
            LinearMovementDirection.Up => Vector2.Up,
            LinearMovementDirection.Down => Vector2.Down,
            LinearMovementDirection.Left => Vector2.Left,
            LinearMovementDirection.Right => Vector2.Right,
            _ => throw new
                ArgumentOutOfRangeException(nameof(_direction), "Unknown direction")

        };
            Parent?.Translate(direction * (float) (delta * _speed));
            
    }
    
    public void StopMoving()
    {
        _isMoving = false;
    }
}


public enum LinearMovementDirection
{
    Up,
    Down,
    Left,
    Right
}