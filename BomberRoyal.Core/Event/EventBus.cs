using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Event
{
    public static class EventBus
    {
        public static readonly Dictionary<string, List<Action<object>>> _listeners = new();

        public static void Subscribe(string eventName, Action<object> listener)
        {
            if (!_listeners.ContainsKey(eventName))
            {
                _listeners[eventName] = new List<Action<object>>();
            }
            _listeners[eventName].Add(listener);
        }
        public static void Unsubscribe(string eventName, Action<object> listener)
        {
            if (_listeners.ContainsKey(eventName))
            {
                _listeners[eventName].Remove(listener);
            }
        }
        public static void Publish(string eventName, object data = null)
        {
            if (_listeners.TryGetValue(eventName, out var listeners))
            {
                
                var copy = listeners.ToList();

                foreach (var listener in copy)
                {
                    listener?.Invoke(data);
                }
            }
        }
    }
}
