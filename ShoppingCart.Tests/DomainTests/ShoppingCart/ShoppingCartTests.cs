using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ShoppingCart.Domain.Product;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Domain.ShoppingCart

{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class ShoppingCartTests
    {
        private ShoppingCart _emptyCart;
        
        #region Products

        private Product.Product _iphoneXr;
        private Product.Product _galaxyBudsPlus;
        private Product.Product _otherIphoneXr;
        private Product.Product _anotherIphoneXr;
        private Product.Product _jblCharge4;
        private Product.Product _airPod;
        private Product.Product _spigenCase;
        private Product.Product _xiaomiDots;

        #endregion
        
        #region Promotions

        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountRate20;
        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountRate10;
        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountRate1;
        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountAmount100;
        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountAmount500;
        private Campaign _campaignForEarphoneCategoryOver1QuantityDiscountAmount50;
        private Campaign _campaignForPhoneCategoryOver1QuantityDiscountRate10;

        #endregion

        [SetUp]
        public void Setup()
        {
            _emptyCart = new ShoppingCart();

            _galaxyBudsPlus = new Product.Product(ProductTitle.CreateFromString("GalaxyBuds+"),
                Price.FromDecimal(2000),
                new Category(CategoryTitle.CreateFromString("Earphone")));

            _airPod = new Product.Product(ProductTitle.CreateFromString("AirPod"),
                Price.FromDecimal(2000),
                new Category(CategoryTitle.CreateFromString("Earphone")));

            _jblCharge4 = new Product.Product(ProductTitle.CreateFromString("JBLCharge4"),
                Price.FromDecimal(1000),
                new Category(CategoryTitle.CreateFromString("Speaker")));

            _otherIphoneXr = new Product.Product(ProductTitle.CreateFromString("IphoneXR"),
                Price.FromDecimal(2000),
                new Category(CategoryTitle.CreateFromString("Phone")));

            _anotherIphoneXr = new Product.Product(ProductTitle.CreateFromString("IphoneXR"),
                Price.FromDecimal(2000),
                new Category(CategoryTitle.CreateFromString("Phone")));

            _iphoneXr = new Product.Product(ProductTitle.CreateFromString("IphoneXR"),
                Price.FromDecimal(2000),
                new Category(CategoryTitle.CreateFromString("Phone")));
            
            _spigenCase = new Product.Product(ProductTitle.CreateFromString("SpigenCase"),
                Price.FromDecimal(100),
                new Category(CategoryTitle.CreateFromString("PhoneCases"),new Category(CategoryTitle.CreateFromString("Phone"))));
            
            
            _xiaomiDots = new Product.Product(ProductTitle.CreateFromString("XiaomiDots"),
                Price.FromDecimal(100),
                new Category(CategoryTitle.CreateFromString("Earphone")));

            _campaignForEarphoneCategoryOver1QuantityDiscountRate20 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 2, DiscountTypeEnum.Rate, 20);
            _campaignForEarphoneCategoryOver1QuantityDiscountRate10 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 1, DiscountTypeEnum.Rate, 10);
            _campaignForEarphoneCategoryOver1QuantityDiscountAmount100 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 1, DiscountTypeEnum.Amount, 100);
            _campaignForEarphoneCategoryOver1QuantityDiscountAmount500= new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 1, DiscountTypeEnum.Amount, 500);
            _campaignForEarphoneCategoryOver1QuantityDiscountAmount50 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 1, DiscountTypeEnum.Amount, 50);
            
            _campaignForEarphoneCategoryOver1QuantityDiscountRate1 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Earphone"), null), 1, DiscountTypeEnum.Rate, 1);
            
            _campaignForPhoneCategoryOver1QuantityDiscountRate10 = new Campaign(
                new Category(CategoryTitle.CreateFromString("Phone"), null), 1, DiscountTypeEnum.Rate, 10);
        }

        #region AddItem
        [Test]
        public void AddItem_ThrowsException_IfQuantityIsNotGreaterThan0()
        {
            //Arrange & Act & Assert
            Assert.That(() => _emptyCart.AddItem(new Product.Product(ProductTitle.CreateFromString("Urun"),
                        Price.FromDecimal(2),
                        new Category(CategoryTitle.CreateFromString("Category"))),
                    0)
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Added quantity must be greater then 0"));
        }

        [Test]
        public void AddItem_CreatesAndAddsItem_IfProductIsNotExistInCart()
        {
            //Arrange
            var shoppingCart = _emptyCart;
            //Act
            shoppingCart.AddItem(_iphoneXr,
                1);
            shoppingCart.AddItem(_galaxyBudsPlus,
                2);
            //Assert 
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(3));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(shoppingCart.FinalPrice.Amount));
            Assert.That(shoppingCart.Items?.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(4000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(2));
        }

        [Test]
        public void AddItem_OnlyIncreaseItemQuantityNotAddsNewItem_IfProductIsExistInCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            //Act
            shoppingCart.AddItem(_iphoneXr,
                1);
            shoppingCart.AddItem(_otherIphoneXr,
                2);
            shoppingCart.AddItem(_anotherIphoneXr,
                2);

            //Assert 
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(5));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_otherIphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_anotherIphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(shoppingCart.FinalPrice.Amount));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(5));
        }
        

        #endregion
     

        #region RemoveItem
        [Test]
        public void RemoveItem_ThrowsException_IfProductIsNotInCart()
        {
            //Arrenge 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                1);
            //Act&Assert
            Assert.That(() => shoppingCart.RemoveItem(_galaxyBudsPlus,
                    2)
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Cant remove not existed product"));
        }

        [Test]
        public void RemoveItem_ThrowsException_IfQuantityIsNotGreaterThan0()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                1);
            //Act&Assert
            Assert.That(() => shoppingCart.RemoveItem(_iphoneXr,
                    0)
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Removed quantity must be greater then 0"));
        }

        [Test]
        public void RemoveItem_DecreaseQuantity_IfProductIsExistInCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);

            shoppingCart.AddItem(_jblCharge4,
                4);

            //Act
            shoppingCart.RemoveItem(_otherIphoneXr, 3);
            //Assert 
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(6));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_jblCharge4)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(8000));
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(shoppingCart.FinalPrice.Amount));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(4000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(4000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Quantity, Is.EqualTo(4));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Price.Amount, Is.EqualTo(4000));
        }

        [Test]
        public void RemoveItem_RemoveItemAndRemoveCampaign_WhenCampaignValidatedItemRemoved()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);
            shoppingCart.ApplyDiscount(new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountRate20});

            //Act
            shoppingCart.RemoveItem(_airPod,3);
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(1));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(null));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(0));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void RemoveItem_RemoveAllItem_IfProductQuantityMatchesToItemQuantity()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);

            shoppingCart.AddItem(_jblCharge4,
                4);

            //Act
            shoppingCart.RemoveItem(_otherIphoneXr, 5);
            //Assert 
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(4));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_jblCharge4)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(4000));
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(shoppingCart.FinalPrice.Amount));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Quantity, Is.EqualTo(4));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Price.Amount, Is.EqualTo(4000));
        }

        [Test]
        public void RemoveItem_RemovesItem_IfProductQuantityGreaterThanItemQuantity()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);

            shoppingCart.AddItem(_jblCharge4,
                4);

            //Act
            shoppingCart.RemoveItem(_otherIphoneXr, 8);
            //Assert 
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(4));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_jblCharge4)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(4000));
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(shoppingCart.FinalPrice.Amount));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Quantity, Is.EqualTo(4));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_jblCharge4)).Price.Amount, Is.EqualTo(4000));
        }
        

        #endregion
     
        
        #region Promotions

        [Test]
        public void ApplyDiscount_ThrowsException_IfCampaignsNull()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyDiscount(null)
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("At list one campaign must be given"));
        }

        [Test]
        public void ApplyDiscount_ThrowsException_IfCampaignIsNotSpecified()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyDiscount(new List<Campaign>())
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("At list one campaign must be given"));
        }


        [Test]
        public void ApplyDiscount_ThrowsInvalidOperationException_IfCampaignsAreNotAvailableBecauseOfCategory()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,
                5);
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyDiscount(new List<Campaign>()
                    {_campaignForEarphoneCategoryOver1QuantityDiscountRate20})
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Campaigns are not available for this cart"));
        }

        [Test]
        public void ApplyDiscount_ThrowsInvalidOperationException_IfCampaignsAreNotAvailableBecauseOfConditionQuantity()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_airPod,
                1);
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyDiscount(new List<Campaign>()
                    {_campaignForEarphoneCategoryOver1QuantityDiscountRate20})
                ,
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Campaigns are not available for this cart"));
        }

        [Test]
        public void ApplyDiscount_AppliesRateDiscountToItemOnCampaignCategory_WhenOneValidDiscountRateCampaignGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountRate20});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(6));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(12000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(8000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(2000));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountRate20));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(2000));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }
        [Test]
        public void ApplyDiscount_AppliesRateDiscountToItemOnCampaignCategory_WhenOneValidDiscountRateCampaignGivenForProductRootCategory()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_spigenCase,
                5);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>() {_campaignForPhoneCategoryOver1QuantityDiscountRate10});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(5));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_spigenCase)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(500));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(450));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_spigenCase)).Price.Amount, Is.EqualTo(500));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_spigenCase)).FinalPrice.Amount, Is.EqualTo(450));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_spigenCase)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForPhoneCategoryOver1QuantityDiscountRate10));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(50));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void
            ApplyDiscount_AppliesAmountDiscountToItemOnCampaignCategory_WhenOneValidDiscountAmountCampaignGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            //Act
            shoppingCart.ApplyDiscount(
                new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountAmount100});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(6));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(12000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(11900));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9900));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountAmount100));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void
            ApplyDiscount_AppliesRateDiscountToMultipleItemOnCampaignCategory_WhenOneValidDiscountRateCampaignGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountRate20});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_airPod)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(14800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(8000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(4800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(1200));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountRate20));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(3200));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void
            ApplyDiscount_AppliesAmountDiscountToMultipleItemOnCampaignCategory_WhenOneValidDiscountAmountCampaignGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);

            //Act
            shoppingCart.ApplyDiscount(
                new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountAmount100});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(17900));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9937.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(62.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(5962.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(37.5));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountAmount100));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void
            ApplyDiscount_AppliesMaxAmountDiscountToMultipleItemOnCampaignCategory_WhenMultipleValidDiscountAmountCampaignGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);

            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>()
            {
                _campaignForEarphoneCategoryOver1QuantityDiscountAmount50,
                _campaignForEarphoneCategoryOver1QuantityDiscountAmount100
            });
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(17900));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9937.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(62.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(5962.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(37.5));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountAmount100));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }


        [Test]
        public void
            ApplyDiscount_AppliesMaxAmountDiscountToMultipleItemOnCampaignCategory_WhenMultipleValidDiscountRateCampaignsGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>()
            {
                _campaignForEarphoneCategoryOver1QuantityDiscountRate20,
                _campaignForEarphoneCategoryOver1QuantityDiscountRate10
            });
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_airPod)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(14800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(8000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(4800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(1200));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountRate20));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(3200));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void
            ApplyDiscount_AppliesMaxAmountDiscountToMultipleItemOnCampaignCategoryForRateCampaign_WhenOneValidDiscountRateAndOneValidDiscountAmountCampaignsGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>()
            {
                _campaignForEarphoneCategoryOver1QuantityDiscountRate20,
                _campaignForEarphoneCategoryOver1QuantityDiscountAmount100
            });
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_airPod)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(14800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(8000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(4800));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(1200));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountRate20));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(3200));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }
        
        [Test]
        public void
            ApplyDiscount_AppliesMaxAmountDiscountToMultipleItemOnCampaignCategoryForAmountCampaign_WhenOneValidDiscountRateAndOneValidDiscountAmountCampaignsGiven()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_galaxyBudsPlus,
                5);
            shoppingCart.AddItem(_iphoneXr, 1);
            shoppingCart.AddItem(_airPod, 3);

            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>()
            {
                _campaignForEarphoneCategoryOver1QuantityDiscountRate1,
                _campaignForEarphoneCategoryOver1QuantityDiscountAmount500
            });
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(9));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(17500));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(2000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9687.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(312.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Quantity, Is.EqualTo(3));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).Price.Amount, Is.EqualTo(6000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).FinalPrice.Amount, Is.EqualTo(5812.5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_airPod)).CampaignDiscountPrice.Amount,
                Is.EqualTo(187.5));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountAmount500));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(500));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }
        
        [Test]
        public void ApplyDiscount_AppliesAmountDiscountWithSumOfItemsAmountToItemOnCampaignCategory_WhenOneValidDiscountAmountCampaignGivenAndDiscountAmountGreaterThanTotalAmount()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_xiaomiDots,
                1);
            //Act
            shoppingCart.ApplyDiscount(new List<Campaign>() {_campaignForEarphoneCategoryOver1QuantityDiscountAmount500});
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(1));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_xiaomiDots)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(100));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(0));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_xiaomiDots)).Quantity, Is.EqualTo(1));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_xiaomiDots)).Price.Amount,
                Is.EqualTo(100));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_xiaomiDots)).FinalPrice.Amount,
                Is.EqualTo(0));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_xiaomiDots)).CampaignDiscountPrice.Amount,
                Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountAmount500));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(100));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.Null);
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(0));
        }

        [Test]
        public void AddCoupon_ThrowsException_IfCouponIsNull()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyCoupon(null),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Applied coupon cannot be null (Parameter 'coupon')"));
        }
        
        [Test]
        public void AddCoupon_ThrowsException_IfCartAlreadyHasCoupon()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,1);
            shoppingCart.ApplyCoupon(new Coupon(DiscountTypeEnum.Amount,Price.FromDecimal(20),20));
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyCoupon(new Coupon(DiscountTypeEnum.Amount,Price.FromDecimal(20),20)),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Only one  coupon allowed to apply"));
        }
        
        [Test]
        public void AddCoupon_ThrowsException_IfCouponIsNotValidForCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,1);
            //Act&&Assert
            Assert.That(() => shoppingCart.ApplyCoupon(new Coupon(DiscountTypeEnum.Amount,Price.FromDecimal(2000000),20)),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Coupon not available for cart"));
        }

        [Test]
        public void AddCoupon_MakesDiscountAmount_IfCouponIsValidForCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            var coupon = new Coupon(DiscountTypeEnum.Amount, Price.FromDecimal(20), 20);
            //Act
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(10));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(20000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(19980));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(9990));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9990));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(0));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(null));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(0));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.EqualTo(coupon));
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(20));
        }
        
        [Test]
        public void AddCoupon_MakesDiscountRate_IfCouponIsValidForCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            var coupon = new Coupon(DiscountTypeEnum.Rate, Price.FromDecimal(20), 10);
            //Act
            shoppingCart.ApplyCoupon(coupon);
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(10));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(20000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(18000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(9000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(9000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(0));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(null));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(0));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.EqualTo(coupon));
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(2000));
        }
        
        
        [Test]
        public void AddCoupon_MakesDiscountRate_IfCouponIsValidForRateCampaignDiscountedCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            var coupon = new Coupon(DiscountTypeEnum.Rate, Price.FromDecimal(20), 10);
            //Act
            shoppingCart.ApplyCoupon(coupon);
            shoppingCart.ApplyDiscount(new List<Campaign>()
            {
                _campaignForEarphoneCategoryOver1QuantityDiscountRate10
            });
            //Assert
            Assert.That(shoppingCart.Items.Count, Is.EqualTo(2));
            Assert.That(shoppingCart.Items.Sum(s => s.Quantity), Is.EqualTo(10));
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_iphoneXr)), Is.Not.Null);
            Assert.That(shoppingCart.Items.FirstOrDefault(s => s.Product.Equals(_galaxyBudsPlus)), Is.Not.Null);
            Assert.That(shoppingCart.Price.Amount, Is.EqualTo(20000));
            Assert.That(shoppingCart.FinalPrice.Amount, Is.EqualTo(17100));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Price.Amount, Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).FinalPrice.Amount, Is.EqualTo(9000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_iphoneXr)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Quantity, Is.EqualTo(5));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).Price.Amount,
                Is.EqualTo(10000));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).FinalPrice.Amount,
                Is.EqualTo(8100));
            Assert.That(shoppingCart.Items.First(s => s.Product.Equals(_galaxyBudsPlus)).CampaignDiscountPrice.Amount,
                Is.EqualTo(1000));
            Assert.That(shoppingCart.AppliedCampaignOnCart,
                Is.EqualTo(_campaignForEarphoneCategoryOver1QuantityDiscountRate10));
            Assert.That(shoppingCart.GetCampaignDiscounts(), Is.EqualTo(1000));
            Assert.That(shoppingCart.AppliedCouponOnCart, Is.EqualTo(coupon));
            Assert.That(shoppingCart.GetCouponDiscounts(), Is.EqualTo(1900));
        }
        #endregion

        #region Delivery

        [Test]
        public void GetDeliveryCost_GetsCalculatedCost_WhenTwoDifferentCategoryTwoDifferentProductAddedToCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            //Act Assert
            Assert.That(shoppingCart.GetDeliveryCost(),Is.EqualTo(10.95));
        }
        
        [Test]
        public void GetDeliveryCost_GetsCalculatedCost_WhenTwoDifferentCategoryThreeDifferentProductAddedToCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            shoppingCart.AddItem(_airPod,5);
            //Act Assert
            Assert.That(shoppingCart.GetDeliveryCost(),Is.EqualTo(12.94));
        }
        
        [Test]
        public void GetDeliveryCost_GetsCalculatedCost_WhenThreeDifferentCategoryThreeDifferentProductAddedToCart()
        {
            //Arrange 
            var shoppingCart = _emptyCart;
            shoppingCart.AddItem(_iphoneXr,5);
            shoppingCart.AddItem(_galaxyBudsPlus,5);
            shoppingCart.AddItem(_jblCharge4,5);
            //Act Assert
            Assert.That(shoppingCart.GetDeliveryCost(),Is.EqualTo(14.93));
        }
        #endregion
        //console
    }
}