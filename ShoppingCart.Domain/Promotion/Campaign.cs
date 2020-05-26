using System;
using System.Collections.Generic;
using System.Globalization;
using ShoppingCart.Core;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Domain.Promotion
{
    public class Campaign : Value
    {
        public int  ItemConditionQuantity  { get;  }
        public double Discount  { get;  }      
        public DiscountTypeEnum DiscountType { get;  }
        public Category Category { get;  }

        public Campaign(Category category, int itemConditionQuantity,DiscountTypeEnum discountType, double discount)
        {
            Category = category;
            DiscountType = discountType;
            Discount = discount;
            ItemConditionQuantity = itemConditionQuantity;
            EnsureValidState();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ItemConditionQuantity;
            yield return Discount;
            yield return DiscountType;
            yield return Category;
        }

        protected sealed override void EnsureValidState()
        {
            if (ItemConditionQuantity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ItemConditionQuantity),"ItemConditionQuantity must be greater than 0");
            }

            if (Category == null)
            {
                throw new ArgumentNullException(nameof(Category),"Category must be specified for campaign");
            }

            if (Discount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Discount),"Discount must be greater than 0 ");
            }
            
        }

        public override string ToString()
        {
            string discountType = DiscountType == DiscountTypeEnum.Amount
                ? nameof(DiscountTypeEnum.Amount)
                : nameof(DiscountTypeEnum.Rate);
            return
                $"Category:{Category.Title},Discount:{Discount.ToString(CultureInfo.InvariantCulture)},DiscountType:{ discountType},ItemConditionQuantity:{ItemConditionQuantity}"; 
        }
    }
}