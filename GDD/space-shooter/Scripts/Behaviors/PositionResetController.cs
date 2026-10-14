using System;
using System.Diagnostics;
using Godot;

namespace SpaceShooter.Scripts.Behaviors;
public sealed partial class PositionResetController : ParentBehavior 
{
    private static SpawnManager? _spawnManager; 
    [Export]
    private float _resetY = 800F; 
    [Export]
    private float _respawnY = -50F; 
    public override void _Ready()
    {
        base._Ready(); 
        _spawnManager ??=
            this.GetNodeOrThrow<SpawnManager>("/root/WorldRoot/SpawnManager"); 
    }
    public override void _Process(double delta)
    {
        if (Parent is null)
        {
            throw new UnreachableException("Should have already thrown in _Ready"); 
        }
        if (Parent.Position.Y < _resetY) 
        {
            return;
        }
        var newX = _spawnManager?.SelectSpawnPosition()
                   ?? throw new InvalidOperationException("SpawnManger is not set");
            
        Parent.Position = new Vector2(newX, _respawnY); 
    }
}