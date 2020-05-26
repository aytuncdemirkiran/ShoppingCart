using System;
using System.Collections.Generic;
using ShoppingCart.Core;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Domain.ShoppingCart
{
    public class ShoppingCartItem : Value
    {
        public ShoppingCartItemId Id { get; }
        public Price Price { get; private set; }
        public Price CampaignDiscountPrice { get; private set; }
        public  Price CouponDiscountPrice { get; private set; }
        public Price FinalPrice { get; private set; }
        public int Quantity { get; private set; }
        public Product.Product Product { get; }


        public ShoppingCartItem(int quantity, Product.Product product)
        {
            Id = new ShoppingCartItemId(Guid.NewGuid());
            
            Price = product!=null?Price.FromDecimal(product.Price.Amount * quantity):null;
            FinalPrice = Price;
            Quantity = quantity;
            Product = product;
            CampaignDiscountPrice = Domain.Price.FromDecimal(0);
            CouponDiscountPrice = Domain.Price.FromDecimal(0);
            Product = product;
            EnsureValidState();
        }
        public void MakeCampaignDiscount(Price discountPrice)
        {
            CampaignDiscountPrice = discountPrice;
            FinalPrice = Price.FromDecimal(FinalPrice.Amount - CampaignDiscountPrice.Amount);
        }
        public void MakeCouponDiscount(Price discountPrice)
        {
            CouponDiscountPrice = discountPrice;
            FinalPrice = Price.FromDecimal(FinalPrice.Amount - CouponDiscountPrice.Amount);
        }
        public void IncreaseQuantity(int quantity)
        {
           Quantity= Quantity + quantity;
           this.Price =Domain.Price.FromDecimal(this.Price.Amount+(Product.Price.Amount * quantity)) ;
        }
        
        public void DecreaseQuantity(int quantity)
        {
            Quantity= Quantity - quantity;
            this.Price =Domain.Price.FromDecimal(this.Price.Amount-(Product.Price.Amount * quantity)) ;
        }
        
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return Product;
        }

        protected sealed override void EnsureValidState()
        {
            if (Product == null)
            {
                throw new ArgumentNullException(nameof(Product), "Product must be specified for cartItem");
            }
            if (Price == null)
            {
                throw new ArgumentNullException(nameof(Price), "TotalPrice cannot be null");
            }
            if (Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "CartItem quantity must be greater than 0");
            }
        }
        
        public void ClearCartItemState()
        {
            CampaignDiscountPrice = Domain.Price.FromDecimal(0);
            CouponDiscountPrice = Domain.Price.FromDecimal(0);
            FinalPrice = this.Price;
        }
    }
}