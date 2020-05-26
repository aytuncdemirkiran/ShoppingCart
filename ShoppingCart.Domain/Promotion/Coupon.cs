using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using ShoppingCart.Core;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Domain.Promotion
{
    public class Coupon :Value
    {
        public DiscountTypeEnum DiscountType { get;  }
        public Price MinimumAmountToUse { get;  }
        public double Discount  { get;  }


        public Coupon(DiscountTypeEnum discountType, Price minimumAmountToUse, double discount)
        {
 
            DiscountType = discountType;
            MinimumAmountToUse = minimumAmountToUse;
            Discount = discount;
            EnsureValidState();
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return DiscountType;
            yield return MinimumAmountToUse;
            yield return Discount;
        }

        

        protected sealed override void EnsureValidState()
        {
            if (Discount <=0)
            {
                throw new ArgumentOutOfRangeException(nameof(Discount),"Coupon Discount must be greater than 0");
            }
            else if (MinimumAmountToUse==null )
            {
                throw new ArgumentNullException(nameof(MinimumAmountToUse),"Coupon minimumTotaltoUse must not be null");
            }
            else if (MinimumAmountToUse.Amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MinimumAmountToUse),"Coupon minimumTotaltoUse must be greater then zero");
            }
        }
        
        public override string ToString()
        {
            string discountType = DiscountType == DiscountTypeEnum.Amount
                ? nameof(DiscountTypeEnum.Amount)
                : nameof(DiscountTypeEnum.Rate);
            return
                $"MinimumTotalToUse:{MinimumAmountToUse.Amount.ToString()},Discount:{Discount.ToString()},DiscountType:{ discountType}";
        }
    }
}