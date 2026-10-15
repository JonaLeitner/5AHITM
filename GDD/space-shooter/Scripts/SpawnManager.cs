using System;
using Godot;
using Spaceshooter.Scripts;
using SpaceShooter.Scripts.Eventing;
using SpawnAxisRange = (float MinX, float MaxX);

namespace SpaceShooter.Scripts;

public sealed partial class SpawnManager : Node2D
{
    private const float SpawnOffset = 80F;
    private const float SpawnYPosition = -50F;

    private int _currentlyActiveEnemies;
    private SpawnAxisRange? _spawnRange;
    private Timer? _spawnTimer;
    private EventManager? _eventManager;

    [Export] private PackedScene? _asteroidPrefab;

    [Export] private int _maxEnemies = 25;

    [Export] private Curve? _spawnRate;

    public override void _Ready()
    {
        var windowWidth = GetViewportRect().Size.X;
        _spawnRange = (SpawnOffset, windowWidth - SpawnOffset);
        
        _currentlyActiveEnemies = 0;

        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _eventManager.Subscribe<Events.EnemyDestroyed>(this, _ => _currentlyActiveEnemies--);
        
        _spawnTimer = new Timer
        {
            WaitTime = 2D,
            OneShot = false,
            Autostart = false
        };
        AddChild(_spawnTimer);
        _spawnTimer.Timeout += HandleTimerTick;
        SetSpawnTimerTimeout(CalcSpawnInterval());
    }

    public override void _ExitTree()
    {
        _eventManager?.Unsubscribe<Events.EnemyDestroyed>(this);
        base._ExitTree();
    }

    private void SetSpawnTimerTimeout(TimeSpan interval)
    {
        if (_spawnTimer is null)
        {
            throw new InvalidOperationException("Spawn timer not set");
        }

        _spawnTimer.WaitTime = interval.TotalSeconds;
        _spawnTimer.Start();
        
        
    }

    private void HandleTimerTick()
    {
        if (_asteroidPrefab is null)
        {
            throw new InvalidOperationException("Asteroid prefab not set");
        }

        if (_currentlyActiveEnemies >= _maxEnemies)
        {
            // GD.Print("Reached asteroid cap");
            return;
        }

        var spawnPos = new Vector2(SelectSpawnPosition(), SpawnYPosition);
        var enemy = _asteroidPrefab.Instantiate<Asteroid>();
        enemy.Position = spawnPos;
        AddChild(enemy);
        _currentlyActiveEnemies++;
        // GD.Print("Active Enemies: " + _currentlyActiveEnemies);
        SetSpawnTimerTimeout(CalcSpawnInterval());

    }


    
    public float SelectSpawnPosition()
    {
        if (_spawnRange is null)
        {
            throw new InvalidOperationException("Spawn range not set");
        }
        
        var rand = Random.Shared.NextSingle();
        var value = rand * (_spawnRange.Value.MaxX - _spawnRange.Value.MinX)
                    + _spawnRange.Value.MinX;
        return value;
    }
    
    public TimeSpan CalcSpawnInterval()
    {
        if (_spawnRate is null)
        {
            throw new InvalidOperationException("Spawn rate not set");
        }

        var xValue = _currentlyActiveEnemies / (float)_maxEnemies;

        var spawnRate = _spawnRate.Sample(xValue);

        var spawnInterval = 2F / spawnRate;

        return TimeSpan.FromSeconds(spawnInterval);
    }
}