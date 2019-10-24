using DnDTracker.Web.Logging;
using DnDTracker.Web.Objects;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Services.Hubs
{
    public class NotificationHub : Hub
    {
        public static HubConnections Connections = new HubConnections();

        public Task Handshake(dynamic handshake)
        {
            try
            {
                Connections.TryAdd(Context.ConnectionId, new HubConnection
                {
                    ConnectionId = Context.ConnectionId,
                    Client = Clients.Caller,
                    UserGuid = handshake.guid
                });

                return Clients.Caller.SendAsync("HandshakeConfirm", new
                {
                    response = "ok",
                    message = $"Handshake confirmed with guid \"{handshake.guid}\""
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to complete handshake with a new connection. Was there a guid included? Error: {ex.Message}", ex);
            }

            return Clients.Caller.SendAsync("HandshakeConfirm", new
            {
                response = "err",
                message = "Failed to confirm handshake."
            });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            HubConnection connection;
            Connections.TryRemove(Context.ConnectionId, out connection);

            return base.OnDisconnectedAsync(exception);
        }

        public static void SendNotificationToAll(object notifyObject)
        {
            foreach (var connection in Connections.Values)
            {
                connection.Client.SendAsync("ReceiveNotification", notifyObject);
            }
        }

        public static void SendNotification(UserObject user, object notifyObject)
        {
            if (Connections.TryGetByUserGuid(user.Guid, out var connection))
            {
                var client = connection.Client;
                client.SendAsync("ReceiveNotification", notifyObject);
            }
        }
    }
}
