using System;
using NUnit.Framework;
using ShoppingCart.Domain.ShoppingCart;

namespace ShoppingCart.Tests.Domain
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class ShoppingCartItemIdTests
    {
        [Test]
        public void ShoppingCartItemId_ThrowsException_IfValueIsGuidEmpty()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCartItemId(Guid.Empty), 
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Value must not be guid.empty (Parameter 'Value')"));
        }
    }
}