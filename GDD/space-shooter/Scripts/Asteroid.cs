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

        
        //not sure where this prints the logs ._.
        Console.WriteLine($"Asteroid hit by laser {laser}");
        
        //prints the log in the GD console
        GD.Print($"Asteroid hit by laser {laser}");
    }
}