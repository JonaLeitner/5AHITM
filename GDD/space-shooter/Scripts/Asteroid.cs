using SpaceShooter.Scripts;
using SpaceShooter.Scripts.Behaviors;
using SpaceShooter.Scripts.Eventing;

namespace Spaceshooter.Scripts;

using Godot;
using System;

public partial class Asteroid : Node2D
{
    private Sprite2D? _mainSprite;
    private AnimatedSprite2D? _explosionSprite;
    private bool _exploding;
    private EventManager? _eventManager;
    


    public override void _Ready()
    {
        _mainSprite = this.GetNodeOrThrow<Sprite2D>("Collider/Sprite");
        _explosionSprite = this.GetNodeOrThrow<AnimatedSprite2D>("Explosion");
        _eventManager = this.GetNodeOrThrow<EventManager>(EventManager.Path);
        
        _eventManager.Subscribe<Events.GameOver>(this, _ =>
        {
            var movementController =
                this.GetNodeOrThrow<LinearMovementController>("LinearMovement");
            movementController.StopMoving();
        });

        _exploding = false;
        _explosionSprite.Hide();

        _explosionSprite.AnimationFinished += HandleExplosionFinished;
    }

    private async void OnCollision(Area2D other)
    {
        try
        {
            if (_exploding)
            {
                return;
            }


            var otherParent = other.GetParent();
            var reason = Events.EnemyDestroyed.Reason.Laser;
            if (otherParent is not Laser)
            {
                if (otherParent is not Player player)
                {
                    return;
                }

                player.TakeDamage();
                reason = Events.EnemyDestroyed.Reason.Player;
            }
            else
            {
                otherParent.Free();
            }

            _exploding = true;
            this.GetNodeOrThrow<LinearMovementController>("LinearMovement").StopMoving();
            this.GetNodeOrThrow<AudioStreamPlayer2D>("ExplosionSound").Play();
            _eventManager?.Broadcast(new Events.EnemyDestroyed(reason));
            // this.GetNodeOrThrow<EventManager>(EventManager.Path)
            //     .Broadcast(new Events.EnemyDestroyed(reason));
            
            
            _explosionSprite?.Show();
            _explosionSprite?.Play();

            await ToSignal(GetTree().CreateTimer(1D), SceneTreeTimer.SignalName.Timeout);

            _mainSprite?.Hide();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in {nameof(OnCollision)}: {ex.Message}");
            GD.Print($"Error in {nameof(OnCollision)}: {ex.Message}");


            // //not sure where this prints the logs ._.
            // Console.WriteLine($"Asteroid hit by laser {laser}");
            //
            // //prints the log in the GD console
            // GD.Print($"Asteroid hit by laser {laser}");
        }
    }

    public override void _ExitTree()
    {
        if (_explosionSprite != null)
        {
            _explosionSprite.AnimationFinished -= HandleExplosionFinished;
        }
        
        _eventManager?.Unsubscribe<Events.GameOver>(this);

        base._ExitTree();
    }

    private void HandleExplosionFinished()
    {
        QueueFree();
    }
}