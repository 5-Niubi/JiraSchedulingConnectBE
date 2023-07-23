using Microsoft.AspNetCore.SignalR;

namespace JiraSchedulingConnectAppService.SignalR
{
    public class SignalRServer : Hub
    {
        public void HasNewData()
        {
            //Clients.All.SendAsync("ReloadProduct");
        }
    }
}
