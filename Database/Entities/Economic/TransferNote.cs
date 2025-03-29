namespace OctopusAPI.Database.Entities.Economic
{
    public class TransferNote
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public Wallet Sender { get; set; }
        public string ReceiverId { get; set; }
        public Wallet Receiver { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime NotedAt { get; set; }
        public string Description { get; set; }
    }
}