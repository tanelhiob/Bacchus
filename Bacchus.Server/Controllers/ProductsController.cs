using Bacchus.Server.Data;
using Bacchus.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bacchus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly BacchusDbContext _dbContext;

        public ProductsController(IHttpClientFactory httpClientFactory, BacchusDbContext dbContext)
        {
            _httpClient = httpClientFactory.CreateClient();
            _dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IList<Product>>> GetProducts()
        {
            var response = await _httpClient.GetAsync("http://uptime-auction-api.azurewebsites.net/api/auction");
            var products = await response.Content.ReadAsAsync<List<Product>>();

            return products;
        }

        [HttpPost("[action]")]
        public async Task PlaceBid(BidDto bidDto)
        {
            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow,
                Amount = bidDto.Amount,
                Username = bidDto.Username,
                ProductId = bidDto.ProductId,
            };

            _dbContext.Add(bid);
            await _dbContext.SaveChangesAsync();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IList<BidDto>>> GetBids()
        {
            var bids = await _dbContext.Bids.ToListAsync();

            return bids
                .Select(bid => new BidDto { ProductId = bid.ProductId, Amount = bid.Amount, Created = bid.Created, Username = bid.Username })
                .ToList();
        }
    }
}
