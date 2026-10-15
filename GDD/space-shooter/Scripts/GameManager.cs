using Godot;
using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Eventing;

public sealed partial class GameManager : Node2D
{
    private EventManager? _eventManager;
    public override void _Ready()
    {
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _eventManager.Subscribe<Events.PlayerLifeLost>(this, HandlePlayerLifeLost); 
    }
    private void HandlePlayerLifeLost(Events.PlayerLifeLost e)
    {
        if (e.LivesRemaining > 0) 
        {
            return;
        }
        _eventManager?.Broadcast(new Events.GameOver()); 
        var player = this.GetNodeOrThrow<Player>("Player"); 
        player.QueueFree(); 
    }
    public override void _ExitTree()
    {
        _eventManager?.Unsubscribe<Events.PlayerLifeLost>(this); 
        base._ExitTree();
    }
}