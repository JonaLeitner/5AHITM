using System;
using Godot;

public abstract partial class ParentBehavior : Node2D 
{
    protected Node2D? Parent { get; private set; } 
    public override void _Ready()
    {
        Parent = GetParent<Node2D>() 
                 ?? throw new InvalidOperationException("No parent found to control"); 
    }
}   