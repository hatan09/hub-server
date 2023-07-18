using Microsoft.AspNetCore.Identity;

namespace HubServerApp.DataProviders;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool? Gender { get; set; }
    public DateTime Birthdate { get; set; }

    //students: added to group, approved by ins, accepted to interview, hired, finished
    //instructor: add to group, inform times
    //recruiter: job accept, number of new application, inform times
    //admin: out time
    public string? Message { get; set; }

    public string? SignalRConnectionId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public virtual ICollection<UserRole> UserRoles { get; } = new HashSet<UserRole>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new HashSet<Conversation>();

    //[JsonIgnore]
    //public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}
