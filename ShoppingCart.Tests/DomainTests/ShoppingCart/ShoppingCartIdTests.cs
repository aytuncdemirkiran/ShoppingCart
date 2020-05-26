using System;
using NUnit.Framework;
using ShoppingCart.Domain.ShoppingCart;

namespace ShoppingCart.Tests.Domain
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    public class ShoppingCartIdTests
    {
        [Test]
        public void ShoppingCartId_ThrowsException_IfValueIsGuidEmpty()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCartId(Guid.Empty), 
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Value must not be guid.empty (Parameter 'Value')"));
        }
    }
}