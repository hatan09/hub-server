using HubServerApp.DataProviders;
using Microsoft.AspNetCore.SignalR;

namespace HubServer;

public class ChatHub : Hub
{
    private readonly UserManager _userManager;
    private readonly ConversationRepository _conversationRepository;

    #region [ CTor ]
    public ChatHub(UserManager userManager, ConversationRepository conversationRepository)
    {
        _userManager = userManager;
        _conversationRepository = conversationRepository;
    }
    #endregion

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("IdentifyUser", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var token = new CancellationToken(default);
        var user = await _userManager.FindBySignalRConnectionId(Context.ConnectionId);
        if (user != null)
        {
            user.SignalRConnectionId = null;
            await _userManager.UpdateAsync(user);
            await Clients.All.SendAsync("UserLogOut", Context.ConnectionId);
            if (user.Conversations != null && user.Conversations.Any())
            {
                foreach (var conversation in user.Conversations)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversation.Id.ToString(), token);
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(int conversationId)
    {
        var token = new CancellationToken(default);
        var conversation = await _conversationRepository.FindByIdAsync(conversationId, token);
        if (conversation != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
        }
    }

    public async Task JoinAllConversations()
    {
        var token = new CancellationToken(default);
        var user = await _userManager.FindBySignalRConnectionId(Context.ConnectionId, token);
        if (user != null && user.Conversations != null && user.Conversations.Any())
        {
            foreach (var conversation in user.Conversations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            };
        }
    }

    public async Task ChatHubUserIndentity(string connectionId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.SignalRConnectionId = connectionId;
            await _userManager.UpdateAsync(user);
        }
    }
    public async Task SendMessage(string mess,
                                  DateTime sentTime,
                                  int conversationId,
                                  string fromUserId,
                                  string toUserId)
    {
        var fromUser = await _userManager.FindBySignalRConnectionIdAndConversationAsync(Context.ConnectionId, conversationId);
        var toUser = await _userManager.FindByIdAsync(toUserId);
        if (fromUser == null || toUser == null)
        {
            return;
        }
        if (!string.IsNullOrEmpty(toUser.SignalRConnectionId) /*fromUser.Conversations != null && fromUser.Conversations.Any()*/)
        {
            await Clients.Client(toUser.SignalRConnectionId).SendAsync("ReceiveMessage", conversationId, fromUserId, mess, sentTime);
            //foreach(var conversation in fromUser.Conversations)
            //{
            //    await Clients.Groups(conversation.Id.ToString()).SendAsync("ReceiveMessage", conversationId, fromUserId, mess, sentTime);
            //}
        }
    }
}
