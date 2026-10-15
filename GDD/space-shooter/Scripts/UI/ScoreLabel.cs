using Godot;
using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Eventing;

public sealed partial class ScoreLabel : Label 
{
    private const string ScoreText = " Score: {0:0000}"; 
    private const int PointsPerEnemy = 10; 
    private EventManager? _eventManager;
    private int _score;
    public override void _Ready()
    {
        base._Ready();
        _score = 0;
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _eventManager.Subscribe<Events.EnemyDestroyed>(this, e => 
        {
            // only award points if the enemy was destroyed by a laser, not after a collision with the player
                GD.Print(e.DestroyReason);
            
            if (e.DestroyReason == Events.EnemyDestroyed.Reason.Laser) 
            {
                _score += PointsPerEnemy; 
                UpdateScore();
            }
        });
        UpdateScore(); 
    }
    public override void _ExitTree()
    {
        _eventManager?.Unsubscribe<Events.EnemyDestroyed>(this); 
        base._ExitTree();
    }
    private void UpdateScore()
    {
        Text = string.Format(ScoreText, _score); 
    }
}