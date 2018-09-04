namespace Eshopworld.Strada.Web
{
    /// <summary>
    ///     OrderRepository provides <see cref="Order" /> CRUD functionality.
    /// </summary>
    public class OrderRepository
    {
        /// <summary>
        ///     Save persists an <see cref="Order" /> instance to DB.
        /// </summary>
        /// <param name="order">The <see cref="Order" /> to persist to DB.</param>
        /// <returns><c>True</c> if the <see cref="Order" /> has been successfully persisted to DB.</returns>
        public bool Save(Order order)
        {
            return true;
        }
    }
}