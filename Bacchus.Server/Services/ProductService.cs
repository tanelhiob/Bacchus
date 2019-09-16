using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bacchus.Server.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IList<Product>> FetchProductsAsync()
        {
            var response = await _httpClient.GetAsync("http://uptime-auction-api.azurewebsites.net/api/auction");
            var products = await response.Content.ReadAsAsync<List<Product>>();

            return products;
        }
    }

    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public DateTimeOffset BiddingEndDate { get; set; }
    }
}
