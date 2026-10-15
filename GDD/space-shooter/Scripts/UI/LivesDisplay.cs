using System;
using Godot;
using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Eventing;

public sealed partial class LivesDisplay : TextureRect 
{
    private EventManager? _eventManager;
    [Export]
    private Texture2D? _oneLifeTexture; 
    [Export]
    private Texture2D? _twoLivesTexture; 
    [Export]
    private Texture2D? _threeLivesTexture; 

    [Export]
    private Texture2D? _zeroLivesTexture; 
    public override void _Ready()
    {
        base._Ready();
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _eventManager.Subscribe<Events.PlayerLifeLost>(this, e => 
        {
            Texture = e.LivesRemaining switch 
            {
                0 => _zeroLivesTexture,
                1 => _oneLifeTexture,
                2 => _twoLivesTexture,
                3 => _threeLivesTexture,
                _ => throw new
                    ArgumentOutOfRangeException(nameof(e.LivesRemaining), e.LivesRemaining, "Invalid number of lives") 
            };
        });
        Texture = _threeLivesTexture; 
    }

public override void _ExitTree()
{
    _eventManager?.Unsubscribe<Events.PlayerLifeLost>(this); 
    base._ExitTree();
}
}