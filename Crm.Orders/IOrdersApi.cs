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
        /// <remarks>
        /// The /orders endpoint retrieves all the orders given the filter for status and email of the user 
        /// </remarks>
        /// <exception cref="Crm.Orders.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="customer">Customer Number (optional)</param>
        /// <param name="orderdate">Order Date (optional)</param>
        /// <param name="salerep">Sales Rep (optional)</param>
        /// <returns>Task of List&lt;Order&gt;</returns>
        Task<List<Order>> GetAsync(string customer = null, string orderdate = null, string salerep = null);

        /// <summary>
        /// Get an order
        /// </summary>
        /// <remarks>
        /// When given the order number, the API will return the individual order. 
        /// </remarks>
        /// <exception cref="Crm.Orders.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="orderNumber">The order number</param>
        /// <returns>Task of Order</returns>
        Task<Order> GetAsync(double? orderNumber);
    }
}
