using System;


namespace BomberRoyal.Core.Event
{
    public interface IEventBus
    {
        void Subscribe<T>(Action<T> listener);
        void Unsubscribe<T>(Action<T> listener);
        void Publish<T>(T eventData);
        void ClearSubscribers(); 
    }
}
