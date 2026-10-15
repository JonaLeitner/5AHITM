namespace SpaceShooter.Scripts.Eventing;
public interface IEvent; 
public static class Events 
{
public readonly record struct EnemyDestroyed(EnemyDestroyed.Reason DestroyReason)
    : IEvent 
{
    public enum Reason 
    {
        Laser,
        Player
    }
}

public readonly record struct GameOver : IEvent;

public readonly record struct PlayerLifeLost(int LivesRemaining) : IEvent; 
}