using System;

namespace ShoppingCart.Core
{
    public abstract class Entity<TId> 
        where TId : Value
    {
        private readonly Action<object> _applier;
        
        public TId Id { get; protected set; }

        protected Entity(Action<object> applier) => _applier = applier;

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            _applier(@event);
        }

    }
}