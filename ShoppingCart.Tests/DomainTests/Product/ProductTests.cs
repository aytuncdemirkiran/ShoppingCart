using System;
using NUnit.Framework;
using ShoppingCart.Domain;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Tests.DomainTests.Product
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void Product_ThrowsException_IfTitleIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new global::ShoppingCart.Domain.Product.Product(null,Price.FromDecimal((decimal)2),new Category(CategoryTitle.CreateFromString("Tekstil"))), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Product's  title must not be null (Parameter 'Title')"));
        }
        
        [Test]
        public void Product_ThrowsException_IfPriceIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new global::ShoppingCart.Domain.Product.Product(ProductTitle.CreateFromString("TShirt"), null,new Category(CategoryTitle.CreateFromString("Tekstil"))), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Product's price must not be null (Parameter 'Price')"));
        }
    }
}