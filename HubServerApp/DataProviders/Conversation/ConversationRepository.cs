using Microsoft.EntityFrameworkCore;

namespace HubServerApp.DataProviders;

public class ConversationRepository
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<Conversation> _dbSet;
    public ConversationRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Conversation>();
    }

    public async Task<Conversation?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _dbSet.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return item;
    }
}
