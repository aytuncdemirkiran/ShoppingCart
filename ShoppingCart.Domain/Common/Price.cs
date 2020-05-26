using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain
{

    public class Price :Value
    {
        public decimal Amount { get; }

        protected Price(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException(
                    "Price cannot be negative",
                    nameof(amount));
            
            Amount = amount;
            EnsureValidState();
        }
        

        public  static Price FromDecimal(decimal amount) =>
            new Price(amount);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
        }

        protected sealed override void EnsureValidState()
        {
            if (Amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Amount),"Amount must be greater then 0");
            }
        }

        public override string ToString()
        {
            return Amount.ToString("0.00");
        }
    }
}