using System;
using NUnit.Framework;
using ShoppingCart.Domain.Product;

namespace ShoppingCart.Tests.Domain.Product
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    [TestFixture]
    public class CategoryTitleTests
    {
        [Test]
        public void ProductTitle_ThrowsException_IfTitleLengthGreaterThan30()
        {
            //Arrange & Act & Assert
            Assert.That(() => CategoryTitle.CreateFromString("asdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdasdas"), 
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.EqualTo("Title cannot be longer that 30 characters (Parameter 'Value')"));
        }
        
        [Test]
        public void ProductTitle_ThrowsException_IfTitleIsNull()
        {
            //Arrange & Act & Assert
            Assert.That(() => CategoryTitle.CreateFromString(null), 
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo("Title cannot be null or empty (Parameter 'Value')"));
        }
    }
}