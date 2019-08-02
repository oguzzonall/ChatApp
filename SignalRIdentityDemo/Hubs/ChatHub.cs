using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using SignalRIdentityDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRIdentityDemo.Hubs
{
    public class ChatHub : Hub
    {
        static List<UserHubModel> onlineUser = new List<UserHubModel>();
        ChatRoomEntities context = new ChatRoomEntities();

        public void sendMessageAll(string message)
        {

            var user = context.AspNetUsers.FirstOrDefault(x => x.UserName == Context.User.Identity.Name);
            if (user != null)
            {
                context.ChatMessages.Add(new ChatMessage
                {
                    Message = message,
                    UserId = user.Id,
                    Date = DateTime.Now
                });
                if (context.SaveChanges() > 0)
                {
                    Clients.Others.GetMessageOther(user.UserName, message);
                    Clients.Caller.GetMessageCaller(message);

                }
            }
        }

        public void sendMessage(string message, string currentReceiverId)
        {
            string userId = Context.User.Identity.GetUserId();
            AspNetUser currentReceiver = context.AspNetUsers.FirstOrDefault(x => x.Id == currentReceiverId);
            ChatBox ctb = context.ChatBoxes.FirstOrDefault(x => x.SenderID == userId && x.ReceiverID == currentReceiver.Id || x.SenderID == currentReceiver.Id && x.ReceiverID == userId);
            if (ctb != null)
            {
                ChatMessage chatmesssage = new ChatMessage
                {
                    Message = message,
                    UserId = userId,
                    ReceiverId = currentReceiverId,
                    ChatBoxID = ctb.Id,
                    Date = DateTime.Now
                };

                context.ChatMessages.Add(chatmesssage);

                if (context.SaveChanges() > 0)
                {
                    if (currentReceiver != null)
                    {
                        UserHubModel uhm = onlineUser.FirstOrDefault(x => x.UserName == currentReceiver.UserName);
                        if (uhm != null)
                        {
                            Clients.Clients(new List<string>()
                        {
                            Context.ConnectionId,uhm.ConnectionId
                        }).Refresh(message, chatmesssage.Date.ToShortDateString(),userId);
                        }
                        else
                        {
                            Clients.Caller.MyRefresh(message, chatmesssage.Date.ToShortDateString());
                        }
                    }
                }
            }
        }

        public void OnlineUsers()
        {
            Clients.All.GetOnlineUsers(onlineUser);
        }

        public override Task OnConnected()
        {
            string username = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;


            if (onlineUser.FirstOrDefault(x => x.UserName == username) == null && !String.IsNullOrEmpty(username))
            {
                UserHubModel userHubModel = new UserHubModel
                {
                    ConnectionId=connectionId,
                    UserName=username
                };
                onlineUser.Add(userHubModel);
            }
            OnlineUsers();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string username = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;


            if (onlineUser.FirstOrDefault(x => x.UserName == username) != null && !String.IsNullOrEmpty(username))
            {
                UserHubModel userHubModel=onlineUser.FirstOrDefault(x => x.UserName == username);
                onlineUser.Remove(userHubModel);
            }
            OnlineUsers();

            return base.OnDisconnected(stopCalled);
        }
    }
}