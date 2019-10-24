using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Services.Hubs
{
    public class HubConnections : ConcurrentDictionary<string, HubConnection>
    {
        public bool TryGetByUserGuid(Guid guid, out HubConnection connection)
        {
            try
            {
                connection = this.First(_ => _.Value.UserGuid == guid.ToString()).Value;
                return true;
            }
            catch (Exception ex)
            {
                connection = null;
                return false;
            }
        }
    }

    public class HubConnection
    {
        public string ConnectionId { get; set; }
        public string UserGuid { get; set; }
        public IClientProxy Client { get; set; }
    }
}
