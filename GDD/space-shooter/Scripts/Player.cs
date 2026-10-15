using Godot;
using System;
using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Eventing;

public partial class Player : Node2D
{
    [Export]
    private float _startX = 570F;
    
    [Export]
    private float _startY = 520F;

    [Export] 
    private float _speed = 300F;

    [Export]
    private float _minY = 400F;

    [Export]
    private float _maxY = 580F;

    [Export]
    private PackedScene? _laserPrefab;

    [Export] 
    private double _shootCooldownSeconds = 0.5D;

    [Export] private Node2D? _damage01;
    [Export] private Node2D? _damage02;

    private const int MaxLives = 3;
    private EventManager? _eventManager;
    private int _lives;

    [Export] private Vector2 _laserSpawnOffset = Vector2.Up * 20F;

    private double _lastShotTime;
    private AudioStreamPlayer2D? _laserSound;
    
    
    
    private static readonly StringName shootInput = "shoot";
    private static readonly StringName upInput = "move_up";
    private static readonly StringName downInput = "move_down";
    private static readonly StringName leftInput = "move_left";
    private static readonly StringName rightInput = "move_right";

    private PlayerBoundaries _boundaries;
    
    
    public override void _Ready()
    {
        Position = new Vector2(_startX, _startY);
        _boundaries = DetermineBoundaries();
        _lastShotTime = 0D;
        _laserSound = GetNode<AudioStreamPlayer2D>("LaserSound");
        _lives = MaxLives;
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        _damage01?.Hide();
        _damage02?.Hide();
        
    }

    public void TakeDamage()
    {
        _lives = Math.Clamp(_lives - 1, 0, MaxLives);
        _eventManager?.Broadcast(new Events.PlayerLifeLost(_lives));
        GD.Print($"Player lost a life, {_lives} remaining");

        switch (_lives)
        {
            case >= MaxLives:
            {
                _damage01?.Hide();
                _damage02?.Hide();

                break;
            }
            case 2:
            {
                _damage01?.Show();
                _damage02?.Hide();
                break;
            }
            default:
            {
                _damage01?.Show();
                _damage02?.Show();
                
                break;
            }
            
        }
    }

    public override void _PhysicsProcess(double delta)
    {

   
        
        // var movementDirection = Vector2.Down;
        // var movement = movementDirection * _speed * (float) delta;
        // Translate(movement);

        Move(delta);
        Shoot();

    }

    public void Shoot()
    {
        if (!Input.IsActionPressed(shootInput))
        {
            return;
        }

        if (_laserPrefab is null)
        {
            throw new InvalidOperationException("Laser prefab not set");
        }

        double now = Time.GetTicksMsec() / 1000D;
        if (now - _lastShotTime < _shootCooldownSeconds)
        {
            return;
        }

        var laser = _laserPrefab.Instantiate<Laser>();
        laser.Position = Position + _laserSpawnOffset;
        GetParent().AddChild(laser);
        _lastShotTime = now;
        _laserSound?.Play();
    }

    
    
    
    private void Move(double delta) 
    {
        var movementDirection = Vector2.Zero; 
        
        if (Input.IsActionPressed(upInput) && Position.Y > _boundaries.MinY) 
        {
            movementDirection += Vector2.Up; 
        }
        if (Input.IsActionPressed(downInput) && Position.Y < _boundaries.MaxY)
        {
            movementDirection += Vector2.Down; 
        }
        if (Input.IsActionPressed(leftInput))
        {
            movementDirection += Vector2.Left; 
        }
        if (Input.IsActionPressed(rightInput))
        {
            movementDirection += Vector2.Right; 
        }
        if (movementDirection == Vector2.Zero) 
        {
            return;
        }
        Translate(movementDirection * (float) (_speed * delta)); 
        
        if (Position.X < _boundaries.MinX) 
        {
            Position = new Vector2(_boundaries.MaxX, Position.Y);
        }
        else if (Position.X > _boundaries.MaxX)
        {
            Position = new Vector2(_boundaries.MinX, Position.Y);
        }
    }

    private PlayerBoundaries DetermineBoundaries()
    {

        const int XOffset = 20;
        
        Vector2 windowSize = GetViewportRect().Size;

        var sprite = this.GetNodeOrThrow<Sprite2D>("Collider/PlayerSprite");
        
        Vector2 textureSize = sprite.Texture.GetSize();
        Vector2 scaledSize = textureSize * sprite.Scale;
        Vector2 halfSize = scaledSize / 2F;


        return new(halfSize.X - XOffset,
            windowSize.X - halfSize.X + XOffset,
            _minY,
            _maxY);
    }
    
    private readonly record struct PlayerBoundaries(float MinX, float MaxX, float MinY,
        float MaxY);

}
