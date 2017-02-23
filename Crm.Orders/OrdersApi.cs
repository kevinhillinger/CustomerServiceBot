using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp;
using Crm.Orders.Client;
using Crm.Orders.Model;
using System.Threading.Tasks;

namespace Crm.Orders
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    [Serializable]
    class OrdersApi : IOrdersApi
    {
        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public ApiConfiguration Configuration { get; set; }

        private ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersApi"/> class.
        /// </summary>
        /// <returns></returns>
        public OrdersApi(ApiConfiguration config)
        {
            this.Configuration = config;

            ExceptionFactory = ApiConfiguration.DefaultExceptionFactory;

            // ensure API client has configuration ready
            if (Configuration.ApiClient.Configuration == null)
            {
                this.Configuration.ApiClient.Configuration = this.Configuration;
            }
        }
        

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public Dictionary<string, string> DefaultHeader()
        {
            return this.Configuration.DefaultHeader;
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        public async Task<List<Order>> GetAsync(QueryOptions queryOptions)
        {
            try
            {
                var localVarResponse = await OrdersGetAsyncWithHttpInfo(queryOptions.AccountNumber, queryOptions.OrderDate);
                return localVarResponse.Data;
            }
            catch //just for poc. catch all is NOT production code
            {
                return new List<Order>();
            }
        }

        private async Task<ApiResponse<List<Order>>> OrdersGetAsyncWithHttpInfo(string accountNumber = null, string orderDate = null)
        {

            var localVarPath = "/orders";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();

            object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json"
            };
            string localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (accountNumber != null) localVarQueryParams.Add("account", Configuration.ApiClient.ParameterToString(accountNumber)); // query parameter
            if (orderDate != null) localVarQueryParams.Add("orderdate", Configuration.ApiClient.ParameterToString(orderDate)); // query parameter

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetAsync", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<List<Order>>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (List<Order>) Configuration.ApiClient.Deserialize(localVarResponse, typeof(List<Order>)));
            
        }

        public async Task<Order> GetAsync(string orderNumber)
        {
            try
            {
                ApiResponse<Order> localVarResponse = await OrdersOrderNumberGetAsyncWithHttpInfo(orderNumber);
                return localVarResponse.Data;
            }
            catch
            {
                return null;
            } 
        }

        /// <summary>
        /// Get an order When given the order number, the API will return the individual order. 
        /// </summary>
        /// <exception cref="IO.Swagger.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="orderNumber">The order number</param>
        /// <returns>Task of ApiResponse (Order)</returns>
        private async Task<ApiResponse<Order>> OrdersOrderNumberGetAsyncWithHttpInfo (string orderNumber)
        {
            // verify the required parameter 'orderNumber' is set
            if (orderNumber == null)
                throw new ApiException(400, "Missing required parameter 'orderNumber' when calling OrdersApi->OrdersOrderNumberGet");

            var localVarPath = "/orders/{orderNumber}";
            var localVarPathParams = new Dictionary<string, string>();
            var localVarQueryParams = new Dictionary<string, string>();
            var localVarHeaderParams = new Dictionary<string, string>(Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<string, string>();
            var localVarFileParams = new Dictionary<string, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            string[] localVarHttpContentTypes = new string[] {
            };
            string localVarHttpContentType = Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            string[] localVarHttpHeaderAccepts = new string[] {
                "application/json"
            };
            string localVarHttpHeaderAccept = Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            // set "format" to json by default
            // e.g. /pet/{petId}.{format} becomes /pet/{petId}.json
            localVarPathParams.Add("format", "json");
            if (orderNumber != null) localVarPathParams.Add("orderNumber", Configuration.ApiClient.ParameterToString(orderNumber)); // path parameter


            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetAsync", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<Order>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Name, x => x.Value.ToString()),
                (Order) Configuration.ApiClient.Deserialize(localVarResponse, typeof(Order)));
            
        }

    }
}
