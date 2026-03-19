using Microsoft.AspNetCore.SignalR;

namespace TechBlogApi.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId,string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage",message);
        }

        public async Task SendCommentNotification(int postId,string comment)
        {
            await Clients.Group($"post-{postId}").SendAsync("ReceviceComment",comment);
        }

        public async Task JoinGroup(int postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,$"post-{postId}");
        }

        public async Task LeaveGroup(int postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,$"post-{postId}");
        }
    }
}