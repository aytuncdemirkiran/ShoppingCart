using System;
using NUnit.Framework;
using ShoppingCart.Domain;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Tests.Domain.Product
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class CategoryTests
    {
        [Test]
        public void Product_ThrowsException_IfTitleIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => new Category(null), 
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("CategoryTitle must specified for category (Parameter 'CategoryTitle')"));
        }
    }
}