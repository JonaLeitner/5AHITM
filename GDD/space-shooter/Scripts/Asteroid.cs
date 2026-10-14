using SpaceShooter.Scripts;

namespace Spaceshooter.Scripts;

using Godot;
using System;

public partial class Asteroid : Node2D
{
    private void onCollision(Area2D other)
    {
        if (other.Owner is not Laser laser)
        {
            return;
        }
        
        Console.WriteLine($"Asteroid hit by laser {laser}");
    }
}