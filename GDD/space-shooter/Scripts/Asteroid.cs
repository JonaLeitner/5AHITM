using SpaceShooter.Scripts;

namespace Spaceshooter.Scripts;

using Godot;
using System;

public partial class Asteroid : Node2D
{

    private Sprite2D? _mainSprite;
    private AnimatedSprite2D? _explosionSprite;
    
    private void onCollision(Area2D other)
    {
        if (other.Owner is not Laser laser)
        {
            return;
        }

        
        //not sure where this prints the logs ._.
        Console.WriteLine($"Asteroid hit by laser {laser}");
        
        //prints the log in the GD console
        GD.Print($"Asteroid hit by laser {laser}");
    }

    public override void _Ready()
    {
        _mainSprite = this.GetNodeOrThrow<Sprite2D>("Collider/Sprite"); 
        _explosionSprite = this.GetNodeOrThrow<AnimatedSprite2D>("Explosion");
    }
}