using System;
using Godot;

namespace SpaceShooter.Scripts;

public static class Extensions
{
    extension(Node self)
    {
        public T GetNodeOrThrow<T>(NodePath path) where T : Node =>
            self.GetNode<T>(path)
            ?? throw new InvalidOperationException($"Unable to locate node of type {typeof(T)} at path {path}");
    }
}