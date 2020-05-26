using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain.Product
{
    
    public class CategoryTitle :Value
    {
        public string Value { get; }

        public  static CategoryTitle CreateFromString(string title)
        {
            return new CategoryTitle(title);
        }

        private CategoryTitle(string value)
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
            if (string.IsNullOrEmpty(Value))
            {
                throw new ArgumentException("Title cannot be null or empty",nameof(Value));
            }
            
            if (Value.Length > 30)
                throw new ArgumentOutOfRangeException(
                    nameof(Value),
                    "Title cannot be longer that 30 characters"
                );
            
        }
    }
}