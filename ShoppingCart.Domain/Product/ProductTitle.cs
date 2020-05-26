using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain.Product
{
    public class ProductTitle :Value
    {
        public string Value { get; }

        public  static ProductTitle CreateFromString(string title)
        {
            return new ProductTitle(title);
        }

        private ProductTitle(string value)
        {
            Value = value;   
            EnsureValidState();
        }
        

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        protected sealed override void EnsureValidState()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new ArgumentException("Title cannot be null or empty",nameof(Value));
            }
            else if (Value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Value),
                    "Title cannot be longer that 100 characters"
                );
            }
        }
    }
}