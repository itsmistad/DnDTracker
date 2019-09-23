using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTracker.Web.Services.Hubs
{
    /// <summary>
    /// This is a generic hub for how server-client communication is handled through SignalR.
    /// </summary>
    public class GenericHub : Hub
    {
        /// <summary>
        /// An asynchrnous generic send method for invoking a method on the client-side.
        /// </summary>
        /// <param name="json">The json string to send to the clients.</param>
        public async Task Send(string json)
        {
            await Clients.All.SendAsync("Receive", json);
        }
    }
}
