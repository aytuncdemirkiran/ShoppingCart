using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain.Product
{
    public class Category :Value
    {
        public  CategoryTitle Title { get; private set; }
        public Category ParentCategory { get; private set; }

        public Category(CategoryTitle title, Category parentCategory=null)
        {
            Title = title;
            ParentCategory = parentCategory;
            EnsureValidState();
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Title;
        }

        protected sealed override void EnsureValidState()
        {
            if (Title == null)
            {
                throw new ArgumentNullException(nameof(CategoryTitle),"CategoryTitle must specified for category");
            }
        }
    }
}