using System;

namespace Bacchus.Server.Data
{
    public class Bid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
    }
}
