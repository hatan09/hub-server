using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HubServerApp.DataProviders;

public class UserManager : UserManager<User>
{
    public UserManager(
        IUserStore<User> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher,
        IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<User>> logger
    ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) { }

    public new async Task<User?> FindByNameAsync(string userName)
    {
        var student = await base.FindByNameAsync(userName);
        if (student is null || student.IsDeleted)
            return null;

        return student;
    }

    public async Task<User?> FindBySignalRConnectionId(string connectionId, CancellationToken cancellationToken = default)
    {
        return await Users.Where(x => !x.IsDeleted && x.SignalRConnectionId != null && x.SignalRConnectionId == connectionId)
            .Include(x => x.Conversations)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> FindBySignalRConnectionIdAndConversationAsync(string connectionId, int conversationId, CancellationToken cancellationToken = default)
    {
        return await Users.Where(x => !x.IsDeleted && x.SignalRConnectionId != null && x.SignalRConnectionId == connectionId)
            .Include(x => x.Conversations.Where(x => x.Id == conversationId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
