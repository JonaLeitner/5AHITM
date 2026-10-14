using System;
using Godot;
using Spaceshooter.Scripts;
using SpawnAxisRange = (float MinX, float MaxX);

namespace SpaceShooter.Scripts;

public sealed partial class SpawnManager : Node2D
{
    private const float SpawnOffset = 80F;
    private const float SpawnYPosition = -50F;
    [Export] private PackedScene? _asteroidPrefab;
    private SpawnAxisRange? _spawnRange;

    public override void _Ready()
    {
        var windowWidth = GetViewportRect().Size.X;
        _spawnRange = (SpawnOffset, windowWidth - SpawnOffset);
        var spawnTimer = new Timer
        {
            WaitTime = 2D,
            OneShot = false,
            Autostart = false
        };
        AddChild(spawnTimer);
        spawnTimer.Timeout += HandleTimerTick;
        spawnTimer.Start();
    }

    private void HandleTimerTick()
    {
        if (_asteroidPrefab is null || _spawnRange is null)
        {
            throw new InvalidOperationException("Spawn configuration incomplete");
        }

        var spawnPos = new Vector2(SelectSpawnPosition(), SpawnYPosition);
        var enemy = _asteroidPrefab.Instantiate<Asteroid>();
        enemy.Position = spawnPos;
        AddChild(enemy);
        return;

        float SelectSpawnPosition()
        {
            var rand = Random.Shared.NextSingle();
            var value = rand * (_spawnRange.Value.MaxX - _spawnRange.Value.MinX)
                        + _spawnRange.Value.MinX;
            return value;
        }
    }
}