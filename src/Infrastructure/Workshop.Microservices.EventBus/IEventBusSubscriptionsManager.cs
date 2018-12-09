using System;
using System.Collections.Generic;
using Workshop.Microservices.EventBus.Abstractions;
using Workshop.Microservices.EventBus.Events;

namespace Workshop.Microservices.EventBus
{
    public interface IEventBusSubscriptionsManager
    {
        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();
    }
}