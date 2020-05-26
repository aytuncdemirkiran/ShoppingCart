using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using ShoppingCart.Core;
using ShoppingCart.Domain.Delivery;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Domain.ShoppingCart
{
    public class ShoppingCart : AggregateRoot<ShoppingCartId>
    {
        #region Properties

        public Price FinalPrice { get; private set; }
        public Price Price { get; private set; }
        private Price CouponDiscountPrice { get; set; }
        private Price CampaignDiscountPrice { get; set; }
        private List<Delivery.Delivery> Deliveries { get; set; }
        public List<ShoppingCartItem> Items { get; private set; }
        public Coupon AppliedCouponOnCart { get; private set; }
        public Campaign AppliedCampaignOnCart { get; private set; }

        #endregion

        public ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
            Deliveries = new List<Delivery.Delivery>();
            Id = new ShoppingCartId(Guid.NewGuid());
            Price = Domain.Price.FromDecimal(0);
            FinalPrice = Domain.Price.FromDecimal(0);
        }
        
        #region Public Methods
        
        public void AddItem(Product.Product product, int quantity)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Added quantity must be greater then 0");
            }

            var item = Items?.FirstOrDefault(s => s.Product.Equals(product));
            //Ürün zaten sepette varsa yeni item ekleme quantity arttır
            if (item != null)
            {
                item.IncreaseQuantity(quantity);
            }
            //Yeni ürünse yeni item ekler
            else
            {
                Items.Add(new ShoppingCartItem(quantity, product));
            }

            Apply(new Events.ItemAdded {Product = product, Quantity = quantity});
        }

        public void RemoveItem(Product.Product product, int quantity)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Removed quantity must be greater then 0");
            }

            var item = Items.FirstOrDefault(s => s.Product.Equals(product));
            if (item == null)
            {
                throw new InvalidOperationException("Cant remove not existed product");
            }
            else
            {
                //Silinen quantity item'a eşit yada büyükse item silinir  , değilse quantity düşürülür
                if (item.Quantity <= quantity)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.DecreaseQuantity(quantity);
                }
            }

            Apply(new Events.ItemRemoved {Product = product, Quantity = quantity});
        }

        public void ApplyDiscount(List<Campaign> campaigns)
        {
            if (campaigns == null || !campaigns.Any() )
            {
                throw new InvalidOperationException("At list one campaign must be given");
            }
            var campaignWithBestBenefit = GetAvailableCampaign(campaigns);
            if (campaignWithBestBenefit == null)
            {
                throw new InvalidOperationException("Campaigns are not available for this cart");
            }
            else if (Equals(campaignWithBestBenefit, AppliedCampaignOnCart))
            {
                throw new InvalidOperationException(
                    "CampaignOnCart hast better benefit then tried to applied campaigns");
            }
            AppliedCampaignOnCart = campaignWithBestBenefit;
            Apply(new Events.DiscountApplied() {Campaign = AppliedCampaignOnCart});
        }

        public void ApplyCoupon(Coupon coupon)
        {
            if (coupon == null)
            {
                throw new ArgumentNullException(nameof(coupon), "Applied coupon cannot be null");
            }

            if (AppliedCouponOnCart != null)
            {
                throw new InvalidOperationException("Only one  coupon allowed to apply");
            }

            if (!CheckCouponAvailability(coupon))
            {
                throw new InvalidOperationException("Coupon not available for cart");
            }

            AppliedCouponOnCart = coupon;
            Apply(new Events.CouponApplied() {Coupon = coupon});
        }

        public decimal GetTotalAmountAfterDiscounts()
        {
            return FinalPrice.Amount;
        }

        public decimal GetCouponDiscounts()
        {
            return CouponDiscountPrice?.Amount ?? 0;
        }

        public decimal GetCampaignDiscounts()
        {
            return CampaignDiscountPrice?.Amount ?? 0;
        }

        public decimal GetDeliveryCost()
        {
            return Deliveries.Sum(s => s.Cost.Amount)+DeliveryConstants.FixedCostAmount ;
        }

        #endregion
        


        #region CartPipeline Related
        
        protected override void When(object @event)
        {
            EvaluateCart();
        }
        private void EvaluateCart()
        {
            ClearCartState();
            Items.ForEach(s => s.ClearCartItemState());
            Price = Price.FromDecimal(Items.Sum(s => s.Price.Amount));
            FinalPrice = Price;
            EvaluateCampaignDiscount();
            EvaluateCouponDiscount();
            EvaluateDelivery();
        }

        private void ClearCartState()
        {
            Deliveries = new List<Delivery.Delivery>();
            FinalPrice = null;
            Price = null;
            CampaignDiscountPrice = null;
            CouponDiscountPrice = null;
        }

        private void EvaluateCampaignDiscount()
        {
            if (AppliedCampaignOnCart != null)
            {
                if (GetAvailableCampaign() != null)
                {
                    var itemsToBeDiscounted =
                        Items.Where(s => s.Product.CheckRootCategories(AppliedCampaignOnCart.Category)).ToList();
                    var sumAmount = itemsToBeDiscounted.Sum(s => s.Price.Amount);
                    if (AppliedCampaignOnCart.DiscountType == DiscountTypeEnum.Rate)
                    {
                        //Distribute to Items
                        foreach (var item in itemsToBeDiscounted)
                        {
                            var discountAmount =
                                item.FinalPrice.Amount * (decimal) AppliedCampaignOnCart.Discount / 100;
                            item.MakeCampaignDiscount(Price.FromDecimal(discountAmount));
                        }

                        CampaignDiscountPrice =
                            Price.FromDecimal(sumAmount * (decimal) AppliedCampaignOnCart.Discount / 100);
                        FinalPrice = Price.FromDecimal(Price.Amount - CampaignDiscountPrice.Amount);
                    }
                    else if (AppliedCampaignOnCart.DiscountType == DiscountTypeEnum.Amount)
                    {
                        //Distribute to Items
                        foreach (var item in itemsToBeDiscounted)
                        {
                            var discountAmount =
                                (item.FinalPrice.Amount * (decimal) AppliedCampaignOnCart.Discount) / sumAmount;
                            item.MakeCampaignDiscount(Price.FromDecimal(discountAmount));
                        }

                        CampaignDiscountPrice = Price.FromDecimal((decimal) AppliedCampaignOnCart.Discount);
                        FinalPrice = Price.FromDecimal(Price.Amount - CampaignDiscountPrice.Amount);
                    }
                }
                else
                {
                    Console.WriteLine(
                        Strings.Format("Kampanya artık geçerli değil: ", AppliedCampaignOnCart.ToString()));
                    AppliedCampaignOnCart = null;
                }
            }
        }

        private Campaign GetAvailableCampaign(List<Campaign> campaigns = null)
        {
            if (campaigns == null && AppliedCampaignOnCart != null)
            {
                campaigns = new List<Campaign>()
                {
                    AppliedCampaignOnCart
                };
            }

            var campaignWithBestBenefit = CampaignDiscountPrice != null ? AppliedCampaignOnCart : null;
            if (campaigns != null && campaigns.Any())
            {
                Price maxCampaignDiscount = CampaignDiscountPrice ?? Price.FromDecimal(0);
                foreach (var campaign in campaigns)
                {
                    var itemsOnCampaignCategory =
                        Items.Where(s => s.Product.CheckRootCategories(campaign.Category)).ToList();
                    if (itemsOnCampaignCategory.Sum(s => s.Quantity) >= campaign.ItemConditionQuantity)
                    {
                        if (campaign.DiscountType == DiscountTypeEnum.Amount)
                        {
                            if (maxCampaignDiscount.Amount < (decimal) campaign.Discount)
                            {
                                maxCampaignDiscount = Price.FromDecimal((decimal) campaign.Discount);
                                campaignWithBestBenefit = campaign;
                            }
                        }
                        else if (campaign.DiscountType == DiscountTypeEnum.Rate)
                        {
                            var totalAmount = itemsOnCampaignCategory.Sum(s => s.Price.Amount);
                            var discountAmount = totalAmount * (decimal) campaign.Discount / 100;
                            if (discountAmount > maxCampaignDiscount.Amount)
                            {
                                maxCampaignDiscount = Price.FromDecimal(discountAmount);
                                campaignWithBestBenefit = campaign;
                            }
                        }
                    }
                }
            }

            return campaignWithBestBenefit;
        }

        private void EvaluateCouponDiscount()
        {
            if (AppliedCouponOnCart != null)
            {
                if (!CheckCouponAvailability(AppliedCouponOnCart))
                {
                    Console.WriteLine(Strings.Format("Kupon artık geçerli değil: ", AppliedCouponOnCart.ToString()));
                    AppliedCouponOnCart = null;
                    return;
                }

                if (AppliedCouponOnCart.DiscountType == DiscountTypeEnum.Rate)
                {
                    CouponDiscountPrice =
                        Price.FromDecimal(FinalPrice.Amount * (decimal) AppliedCouponOnCart.Discount / 100);
                    FinalPrice = Price.FromDecimal(FinalPrice.Amount - CouponDiscountPrice.Amount);
                    //Distribute to Items
                    foreach (var item in Items)
                    {
                        var discountAmount = item.FinalPrice.Amount * (decimal) AppliedCouponOnCart.Discount / 100;
                        item.MakeCouponDiscount(Price.FromDecimal(discountAmount));
                    }
                }
                else if (AppliedCouponOnCart.DiscountType == DiscountTypeEnum.Amount)
                {
                    CouponDiscountPrice = Price.FromDecimal((decimal) AppliedCouponOnCart.Discount);
                    //Distribute to Items
                    foreach (var item in Items)
                    {
                        var discountAmount = (item.FinalPrice.Amount * (decimal) AppliedCouponOnCart.Discount) /
                                             FinalPrice.Amount;
                        item.MakeCouponDiscount(Price.FromDecimal(discountAmount));
                    }
                    FinalPrice = Price.FromDecimal(FinalPrice.Amount - CouponDiscountPrice.Amount);

                }
            }
        }

        private bool CheckCouponAvailability(Coupon coupon)
        {
            if (coupon == null) return false;
            return coupon.MinimumAmountToUse.Amount <= FinalPrice.Amount;
        }

        private void EvaluateDelivery()
        {
            var categories = Items.Select(x => x.Product.Category).Distinct();

            foreach (var category in categories)
            {
                Deliveries.Add(new Delivery.Delivery(
                    Items.Where(s => s.Product.Category.Equals(category)).Select(x => x.Id).ToList()));
            }
        }

        #endregion

        protected override void EnsureValidState()
        {
            if (Items.Any())
            {
                if (FinalPrice.Amount > this.Price.Amount)
                {
                    throw new InvalidOperationException("FinalPrice cannot be greater than price");
                }
                
                if (GetDeliveryCost() < DeliveryConstants.FixedCostAmount)
                {
                    throw new InvalidOperationException("Delivery cost cannot be less than FixedCost");
                }
                
                if (AppliedCouponOnCart != null)
                {
                    if (FinalPrice.Amount == this.Price.Amount)
                    {
                        throw new InvalidOperationException("Coupon  applied basket's shopping cart price must be greater than  finalPrice");
                    }

                    if (CouponDiscountPrice?.Amount==0 )
                    {
                        throw new InvalidOperationException("Coupon  applied basket's CouponDiscountPrice cannot be 0");
                    }
                }
                
                if (AppliedCampaignOnCart !=null )
                {
                    if (FinalPrice.Amount == this.Price.Amount)
                    {
                        throw new InvalidOperationException("Campaign  applied basket's shopping cart price must be greater than  finalPrice");
                    }

                    if (CampaignDiscountPrice?.Amount==0)
                    {
                        throw new InvalidOperationException("Campaign  applied basket's CampaignDiscountPrice cannot be 0");
                    }
                }
            }

        }

        public void Print()
        {
            if (Items != null && Items.Any())
            {
                var categories = Items.Select(x => x.Product.Category).Distinct();
                Console.WriteLine("\n CART PRINT");
                foreach (var category in categories)
                {
                    var itemsOnCategory = Items.Where(s => s.Product.Category.Equals(category)).ToList();
                    Console.WriteLine($"Product on {category.Title.Value} Category");
                    foreach (var item in itemsOnCategory)
                    {
                        Console.WriteLine("ProductName : {0}  ;" +
                                          " Quantity :{1} ;" +
                                          " UnitPrice:{2} ;" +
                                          " TotalPrice:{3} ;" +
                                          " FinalPrice:{4};" +
                                          " TotalDiscount:{5} ;" +
                                          " CouponDiscount:{6} ; " +
                                          " CampaignDiscount:{7} ", 
                            item.Product.Title,
                            item.Quantity,
                            item.Product.Price.Amount,
                            item.Price.Amount,
                            item.FinalPrice.Amount,
                            Domain.Price.FromDecimal(item.Price.Amount-item.FinalPrice.Amount).Amount,
                            item.CouponDiscountPrice?.Amount, 
                            item.CampaignDiscountPrice?.Amount);

                    }
                }
                                    
                Console.WriteLine("\n CHANGES  on Cart");
                this.GetChanges().ToList().ForEach(s=>Console.WriteLine(s));
            }
        }

    }
}