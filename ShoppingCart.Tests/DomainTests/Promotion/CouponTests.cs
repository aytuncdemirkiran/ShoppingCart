using System;
using NUnit.Framework;
using ShoppingCart.Domain;
using ShoppingCart.Domain.Promotion;

namespace ShoppingCart.Tests.Domain.Promotion
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class CouponTests
    {
        [Test]
        public void Coupon_ThrowsException_IfMinimumTotalToUseIsNotGreaterThan0()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Coupon(DiscountTypeEnum.Amount,Price.FromDecimal(0), 20 ), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("Coupon minimumTotaltoUse must be greater then zero (Parameter 'MinimumAmountToUse')"));
        }
        
        [Test]
        public void Coupon_ThrowsException_IfMinimumTotalToUseIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Coupon(DiscountTypeEnum.Amount,null,20 ), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Coupon minimumTotaltoUse must not be null (Parameter 'MinimumAmountToUse')"));
        }

        [Test]
        public void Coupon_ThrowsException_IfDiscountNotGreaterThan0()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Coupon(DiscountTypeEnum.Amount,Price.FromDecimal(22),0 ), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("Coupon Discount must be greater than 0 (Parameter 'Discount')"));
        }
        
    }
}