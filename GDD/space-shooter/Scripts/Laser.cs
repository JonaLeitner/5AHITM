using System;
using Godot;

namespace SpaceShooter.Scripts;

public sealed partial class Laser : Node2D
{
    private static float? _destructionY;


    public override void _Ready()
    {
        _destructionY ??= CalcDestructionY();
        
        return;

        float CalcDestructionY()
        {

            var sprite = this.GetNodeOrThrow<Sprite2D>("Collider/LaserSprite");
            
            var textureHeight = sprite.Texture.GetSize().Y * Scale.Y;

            return -1 * (textureHeight * 2F + 1F);
        }



    }



    public override void _Process(double delta)
    {

        if (Position.Y < (_destructionY ?? throw new InvalidOperationException("Destruction Y not set")))
        {
            QueueFree();
        }

    }
}
