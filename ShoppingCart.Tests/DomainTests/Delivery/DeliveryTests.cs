using System;
using System.Collections.Generic;
using NUnit.Framework;
using ShoppingCart.Domain;
using ShoppingCart.Domain.ShoppingCart;

namespace ShoppingCart.Tests.Domain.Delivery
{
    // Naming Convention MethodName_ExpectedBehavior_StateUnderTest
    public class DeliveryTests
    {
        [Test]
        public void Delivery_ThrowsException_ItemIdsNotSpecified()
        {
            //Arrange & Act & Assert
            Assert.That(() => new ShoppingCart.Domain.Delivery.Delivery(new List<ShoppingCartItemId>()), 
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("DeliveryCost can not be less then fixedcost"));

        }

        [Test]
        public void EvaluateCost_EvaluatesAccordingToFormula_IfStateIsValid()
        {
            //Arrenge & Act 
            var delivery = new ShoppingCart.Domain.Delivery.Delivery(new List<ShoppingCartItemId>()
            {
                new ShoppingCartItemId(Guid.NewGuid()),
                new ShoppingCartItemId(Guid.NewGuid())
            });
            
            //Assert 
            Assert.That(delivery.Cost.Amount,Is.EqualTo((decimal)5.97));
        }
    }
}