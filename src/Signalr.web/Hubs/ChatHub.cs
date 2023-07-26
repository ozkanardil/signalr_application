using Microsoft.AspNetCore.SignalR;

namespace Signalr.web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        private static Dictionary<string, string> _clientConnectionMap = new Dictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            // Generate a unique identifier for the connected client
            var connectionId = Context.ConnectionId;

            // Store the connectionId along with the client's name or any other identifier
            // In this example, we'll use the name provided by the client as the identifier.
            var name = Context.GetHttpContext().Request.Query["name"].ToString();

            lock (_clientConnectionMap)
            {
                _clientConnectionMap[connectionId] = name;
            }

            var destopConnId = _clientConnectionMap.FirstOrDefault(x => x.Value == "desktop").Key;
            Clients.Client(destopConnId).SendAsync("ReceiveClients", _clientConnectionMap);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Remove the client's entry from the dictionary on disconnection
            var connectionId = Context.ConnectionId;

            lock (_clientConnectionMap)
            {
                if (_clientConnectionMap.ContainsKey(connectionId))
                {
                    _clientConnectionMap.Remove(connectionId);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToClient(string clientName, string message)
        {
            // Find the connectionId for the specific client using the provided clientName
            var connectionId = _clientConnectionMap.FirstOrDefault(x => x.Value == clientName).Key;

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }
}
