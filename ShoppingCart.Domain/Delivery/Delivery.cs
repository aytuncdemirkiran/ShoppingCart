using System;
using System.Collections.Generic;
using System.Linq;
using ShoppingCart.Core;
using ShoppingCart.Domain.Product;
using ShoppingCart.Domain.ShoppingCart;

namespace ShoppingCart.Domain.Delivery
{
    public class Delivery :Value
    {
        public List<ShoppingCartItemId> ItemIds { get; private set; }
        public Price Cost { get; private set; }

        public Delivery(List<ShoppingCartItemId> itemIds)
        {
            ItemIds = itemIds;
            EvaluateCost();
            EnsureValidState();
        }


        public void EvaluateCost()
        {
            if (ItemIds.Any())
            {
                Cost=Price.FromDecimal(DeliveryConstants.CostPerDelivery +
                                       (ItemIds.Count * DeliveryConstants.CostPerProduct));
            }
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ItemIds;
            yield return Cost;
        }

        protected sealed override void EnsureValidState()
        {
            if (Cost==null || Cost.Amount<DeliveryConstants.FixedCostAmount)
            {
                throw new InvalidOperationException("DeliveryCost can not be less then fixedcost");
            }
        }
    }
}