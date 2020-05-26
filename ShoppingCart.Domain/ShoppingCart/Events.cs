using System;
using System.Collections.Generic;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Domain.ShoppingCart
{
    public static class Events
    {
        public class ItemAdded
        {
            public Product.Product Product { get; set; }
            public int Quantity { get; set; }

            public override string ToString()
            {
                return $"Item Added Quantity:{Quantity} Product:{Product}";
            }
        }
        
        public class ItemRemoved
        {
            public Product.Product Product { get; set; }
            public int Quantity { get; set; }
            
            public override string ToString()
            {
                return $"Item Removed Quantity:{Quantity} Product:{Product}";
            }
        }

        public class DiscountApplied
        {
            public Campaign Campaign { get; set; }
            
            public override string ToString()
            {
                return $"DiscountApplied  Campaign:{Campaign} ";
            }
        }
        
        public class CouponApplied
        {
            public Coupon Coupon { get; set; }
            public override string ToString()
            {
                return $"CouponApplied  Campaign:{Coupon} ";
            }
        }
    }
}