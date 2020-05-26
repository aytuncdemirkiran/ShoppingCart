using System;
using NUnit.Framework;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Tests.DomainTests.Product
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class ProductTitleTests
    {
        [Test]
        public void ProductTitle_ThrowsException_IfTitleLengthGreaterThan100()
        {
            //Arrange & Act & Assert
            Assert.That(() => ProductTitle.CreateFromString("asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdas"), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("Title cannot be longer that 100 characters (Parameter 'Value')"));
        }
        
        [Test]
        public void ProductTitle_ThrowsException_IfTitleIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => ProductTitle.CreateFromString(null), 
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Title cannot be null or empty (Parameter 'Value')"));
        }
        
    }
}