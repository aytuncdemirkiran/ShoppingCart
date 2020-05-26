using System;
using NUnit.Framework;
using ShoppingCart.Domain;
using ShoppingCart.Domain.Product;
using ShoppingCart.Domain.ShoppingCart;

namespace ShoppingCart.Tests.DomainTests.ShoppingCart
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class ShoppingCartItemTests
    {
        [Test]
        public void ShoppingCartItem_ThrowsException_IfQuantityNotGreaterThanZero()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCartItem(0,
                    new global::ShoppingCart.Domain.Product.Product(ProductTitle.CreateFromString("Product"),Price.FromDecimal(2),
                        new Category(CategoryTitle.CreateFromString("Category"))  )), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("CartItem quantity must be greater than 0 (Parameter 'Quantity')"));
        }
        
        [Test]
        public void ShoppingCartItem_ThrowsException_IfPriceIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCartItem(2,
                    new global::ShoppingCart.Domain.Product.Product(ProductTitle.CreateFromString("Product"),null,
                        new Category(CategoryTitle.CreateFromString("Category"))  )), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Product's price must not be null (Parameter 'Price')"));
        }
        
        [Test]
        public void ShoppingCartItem_ThrowsException_IfProductIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCartItem(2,null), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Product must be specified for cartItem (Parameter 'Product')"));
        }
    }
}