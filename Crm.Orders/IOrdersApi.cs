using Crm.Orders.Client;
using Crm.Orders.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crm.Orders
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IOrdersApi : IApiAccessor
    {
        /// <summary>
        /// Orders
        /// </summary>
        Task<List<Order>> GetAsync(QueryOptions options);

        /// <summary>
        /// Get an order
        /// </summary>
        Task<Order> GetAsync(string orderNumber);
    }
}
