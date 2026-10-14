using SpaceShooter.Scripts;

namespace Spaceshooter.Scripts;

using Godot;
using System;

public partial class Asteroid : Node2D
{

    private Sprite2D? _mainSprite;
    private AnimatedSprite2D? _explosionSprite;
    private bool _exploding;
    
    
    public override void _Ready()
    {
        _mainSprite = this.GetNodeOrThrow<Sprite2D>("Collider/Sprite"); 
        _explosionSprite = this.GetNodeOrThrow<AnimatedSprite2D>("Explosion");

        _exploding = false;
        _explosionSprite.Hide();
        
        _explosionSprite.AnimationFinished += HandleExplosionFinished;
    }
    private void onCollision(Area2D other)
    {
        if ( _exploding || other.Owner is not Laser)
        {
            return;
        }
        
        other.Owner.Free();

        _exploding = true;
        _mainSprite?.Hide();
        _explosionSprite?.Show();
        _explosionSprite?.Play();
        

        
        // //not sure where this prints the logs ._.
        // Console.WriteLine($"Asteroid hit by laser {laser}");
        //
        // //prints the log in the GD console
        // GD.Print($"Asteroid hit by laser {laser}");
    }
    
    public override void _ExitTree()
    {
        if (_explosionSprite != null) 
        {
            _explosionSprite.AnimationFinished -= HandleExplosionFinished; 
        }
        base._ExitTree(); 
    }
    private void HandleExplosionFinished() 
    {
        QueueFree(); 
    }
    
}