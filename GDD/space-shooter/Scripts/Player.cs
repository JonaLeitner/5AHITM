using Godot;
using System;
using SpaceShooter.Scripts;

public partial class Player : Sprite2D
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


    }

    public override void _Process(double delta)
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
        Vector2 windowSize = GetViewportRect().Size;
        Vector2 textureSize = Texture.GetSize();
        Vector2 scaledSize = textureSize * Scale;
        Vector2 halfSize = scaledSize / 2F;


        return new(halfSize.X,
            windowSize.X - halfSize.X,
            _minY,
            _maxY);
    }
    
    private readonly record struct PlayerBoundaries(float MinX, float MaxX, float MinY,
        float MaxY);

}
