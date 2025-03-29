namespace OctopusAPI.Database.Entities.Economic
{
    public class Wallet
    {
        public string Id { get; set; }
        public int OwnerId { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<TransferNote> RecivedTransferNotes { get; set; }
        public ICollection<TransferNote> SentTransferNotes { get; set; }
    }
}