using System;
using System.Collections.Generic;
using ShoppingCart.Domain;
using ShoppingCart.Domain.Product;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Category
            var foodCategory=new Category(CategoryTitle.CreateFromString("Food"));
            var phoneCategory=new Category(CategoryTitle.CreateFromString("Phone"));
            //Products
            var apple=new Product(ProductTitle.CreateFromString("Apple"),Price.FromDecimal((decimal)100.0),foodCategory);
            var almond=new Product(ProductTitle.CreateFromString("Almond"),Price.FromDecimal((decimal)100.0),foodCategory);
            var iphone=new Product(ProductTitle.CreateFromString("Iphone"),Price.FromDecimal((decimal)10000.0),phoneCategory);
            //ShoppingCart Init and AddItems
            var shoppingCart=new Domain.ShoppingCart.ShoppingCart();
            shoppingCart.AddItem(apple,2 );
            shoppingCart.AddItem(almond,3 );
            shoppingCart.AddItem(iphone,1);
            //Create and Apply Campaigns to Cart 
            var campaigns = new List<Campaign>()
            {
                new Campaign(foodCategory,3,DiscountTypeEnum.Rate,20),
                new Campaign(foodCategory,5,DiscountTypeEnum.Rate,50),
                new Campaign(foodCategory,5,DiscountTypeEnum.Amount,5.0)
            };
            shoppingCart.ApplyDiscount(campaigns);
            //Create Coupon and apply
            var coupon=new Coupon(DiscountTypeEnum.Rate,Price.FromDecimal((decimal)100.00),10 );
            shoppingCart.ApplyCoupon(coupon);

            System.Console.WriteLine($"Total Amount After Discounts:{shoppingCart.GetTotalAmountAfterDiscounts()}");
            System.Console.WriteLine($"Total Discount By Coupon:{shoppingCart.GetCouponDiscounts()}");
            System.Console.WriteLine($"Total Discount By Campaign:{shoppingCart.GetCampaignDiscounts()}");
            System.Console.WriteLine($"Delivery Cost:{shoppingCart.GetDeliveryCost()}");

            shoppingCart.Print();
            
            System.Console.ReadLine();
        }
    }
}