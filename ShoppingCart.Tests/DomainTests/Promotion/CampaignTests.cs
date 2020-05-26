using System;
using NUnit.Framework;
using ShoppingCart.Domain.Product;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Tests.Domain.Promotion
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class CampaignTests
    {
        [Test]
        public void Campaign_ThrowsException_IfItemConditionQuantity()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Campaign(new Category(CategoryTitle.CreateFromString("Tekstil")),0,DiscountTypeEnum.Amount,20 ), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("ItemConditionQuantity must be greater than 0 (Parameter 'ItemConditionQuantity')"));
        }
        
        [Test]
        public void Campaign_ThrowsException_IfCategoryIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Campaign(null,1,DiscountTypeEnum.Amount,20 ), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Category must be specified for campaign (Parameter 'Category')"));
        }
        
        [Test]
        public void Campaign_ThrowsException_IFDiscountNotGreaterThan0()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Campaign(new Category(CategoryTitle.CreateFromString("Tekstil")),1,DiscountTypeEnum.Amount,0), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("Discount must be greater than 0  (Parameter 'Discount')"));
        }
    }
}