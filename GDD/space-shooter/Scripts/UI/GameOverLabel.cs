using Godot;
using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Eventing;

public sealed partial class GameOverLabel : Label
{
    [Export] private float _blinkIntervalSeconds = 1.5F;
    private bool _displayLabel;
    private EventManager? _eventManager;
    private bool _isGameOver;
    private double _timeLastBlinkChange;

    public override void _Ready()
    {
        base._Ready();
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _eventManager.Subscribe<Events.GameOver>(this, _ =>
        {
            GD.Print("GameOverLabel: I received the event!");

            _isGameOver = true;
        });

        _isGameOver = false;
        _displayLabel = false;
        _timeLastBlinkChange = 0;
        Hide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!_isGameOver)
        {
            return;
        }

        var now = Time.GetTicksMsec() / 1000D;
        if (now - _timeLastBlinkChange < _blinkIntervalSeconds)
        {
            return;
        }

        _displayLabel = !_displayLabel;
        _timeLastBlinkChange = now;
        if (_displayLabel)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public override void _ExitTree()
    {
        _eventManager?.Unsubscribe<Events.GameOver>(this);
        base._ExitTree();
    }
}