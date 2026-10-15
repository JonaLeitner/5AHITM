using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Godot;

namespace SpaceShooter.Scripts.Eventing;

public sealed partial class EventManager : Node
{
    // This code is optimized for performance not readability (hence the object and casts)
    // => do not take this file as an example for clean code!
    private readonly ConcurrentDictionary<Type, object> _channels = new();
    public static NodePath Path { get; } = new("/root/EventManager");

    public void Broadcast<T>(T message) where T : IEvent
    {
        if (_channels.TryGetValue(typeof(T), out object? channel))
        {
            ((Channel<T>)channel).Broadcast(message);
        }
    }

    public void Subscribe<T>(Node subscriber, Action<T> callBack) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(callBack);
        var channel = (Channel<T>)_channels.GetOrAdd(typeof(T), static _ => new
            Channel<T>()); 
        channel.Set(subscriber, callBack);
    }

    public void Unsubscribe<T>(Node subscriber) where T : IEvent
    {
        if (_channels.TryGetValue(typeof(T), out object? channel))
        {
            ((Channel<T>)channel).Remove(subscriber);
        }
    }

    private sealed class Channel<T> where T : IEvent
    {
        private readonly Dictionary<Node, Action<T>> _callBacks =
            new(ReferenceEqualityComparer.Instance);

        private readonly Lock _mutex = new();
        private volatile Action<T>[] _handlers = [];

        public void Broadcast(T message)
        {
            Action<T>[] handlers = _handlers;
            foreach (Action<T> callBack in handlers)
            {
                callBack(message);
            }
        }

        public void Set(Node subscriber, Action<T> callBack)
        {
            lock (_mutex)
            {
                _callBacks[subscriber] = callBack;
                Publish();
            }
        }

        public void Remove(Node subscriber)
        {
            lock (_mutex)
            {
                if (_callBacks.Remove(subscriber))
                {
                    Publish();
                }
            }
        }

        private void Publish() => _handlers = _callBacks.Count == 0
            ? []
            :
            [
                ..
                _callBacks.Values
            ];
    }
}