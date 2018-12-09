using System;
using System.Collections.Generic;
using System.Linq;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            DoAddSubscription(typeof(TH), eventName, isDynamic: false);
            _eventTypes.Add(typeof(T));
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                                            $"Handler Type {handlerType.Name} already registered for '{eventName}'",
                                            nameof(handlerType));
            }

            _handlers[eventName]
                .Add(isDynamic
                         ? SubscriptionInfo.Dynamic(handlerType)
                         : SubscriptionInfo.Typed(handlerType));
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}