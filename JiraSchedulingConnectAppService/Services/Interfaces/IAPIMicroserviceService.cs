namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAPIMicroserviceService
    {
        public Task<HttpResponseMessage> Get(string url);
        public Task<HttpResponseMessage> Post(string url, dynamic contentObject);
    }
}
