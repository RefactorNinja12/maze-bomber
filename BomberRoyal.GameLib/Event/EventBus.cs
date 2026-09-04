using System;
using System.Collections.Generic;


namespace BomberRoyal.Core.Event
{
    // https://discussions.unity.com/t/why-dont-you-use-eventbus/940577
    // En enkel event-bus som låter objekt kommunicera utan direkta beroenden.
    public class EventBus : IEventBus
    {
        // Håller en lista av lyssnare för varje event-typ 
        private readonly Dictionary<Type, List<Delegate>> _listeners = new();

        // Registrerar en metod som lyssnar på ett visst event.
        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (!_listeners.ContainsKey(type))
                _listeners[type] = new List<Delegate>();
            _listeners[type].Add(listener);
        }
        // Tar bort en tidigare registrerad lyssnare.

        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_listeners.ContainsKey(type))
                _listeners[type].Remove(listener);
        }
        // Skickar ut ett event till alla prenumeranter av den typen.
        public void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (_listeners.TryGetValue(type, out var listeners))
            {
                
                foreach (var listener in listeners.ToArray())
                    (listener as Action<T>)?.Invoke(eventData);
            }
        }
        // Tar bort alla registrerade lyssnare t.ex. vid reset eller avslut.
        public void ClearSubscribers() => _listeners.Clear();
    }
}
