namespace Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
