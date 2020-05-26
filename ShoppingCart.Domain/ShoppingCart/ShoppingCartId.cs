using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain.ShoppingCart
{
    public class ShoppingCartId :Value
    {
        public Guid Value { get;  }

        public ShoppingCartId(Guid value)
        {
            Value = value;
            EnsureValidState();
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        protected sealed override void EnsureValidState()
        {
            if (Value == Guid.Empty)
            {
                throw  new ArgumentException("Value must not be guid.empty",nameof(Value));
            }
        }
    }
}