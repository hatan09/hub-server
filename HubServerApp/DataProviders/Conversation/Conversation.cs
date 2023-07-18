using System.ComponentModel.DataAnnotations;

namespace HubServerApp.DataProviders;

public class Conversation
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Pin { get; set; }

    public DateTime? LastMessageTime { get; set; }
    public string? LastMessage { get; set; }
    public string? LastSender { get; set; }

    public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    //public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}
