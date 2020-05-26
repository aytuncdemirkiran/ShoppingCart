using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core
{
    public abstract class AggregateRoot<TId>
    {
        public TId Id { get; protected set; }

        protected abstract void When(object @event);
        
        private readonly List<object> _changes;

        protected AggregateRoot() => _changes = new List<object>();

        protected void Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            Console.WriteLine(@event);
            _changes.Add(@event);
        }

        public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

        public void ClearChanges() => _changes.Clear();

        protected abstract void EnsureValidState();



    }
}