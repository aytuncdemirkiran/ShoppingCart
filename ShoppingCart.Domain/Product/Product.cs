using System;
using System.Collections.Generic;
using ShoppingCart.Core;

namespace ShoppingCart.Domain.Product
{
    public class Product : Value
    {
        public ProductTitle Title { get; }
        public Price Price { get; }

        public Category Category { get; }


        public Product(ProductTitle title, Price price, Category category)
        {
            Title = title;
            Price = price;
            Category = category;
            EnsureValidState();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Title;
            yield return Price;
            yield return Category;
        }

        public override string ToString()
        {
            return string.Format("Title:{0} - Category:{1} - Price {2}",Title,Category.Title.Value,this.Price);
        }

        public bool CheckRootCategories(Category category)
        {
            Category parentCategory = Category;
            while (parentCategory != null)
            {
                if (category.Equals(parentCategory))
                {
                    return true;
                }
                parentCategory = parentCategory.ParentCategory;
            }

            return false;
        }

        protected sealed override void EnsureValidState()
        {
            if (Title == null)
            {
                throw new ArgumentNullException(
                    nameof(Title),
                    "Product's  title must not be null"
                );
            }

            if (this.Price == null)
            {
                throw new ArgumentNullException(
                    nameof(this.Price),
                    "Product's price must not be null"
                );
            }
        }
    }
}