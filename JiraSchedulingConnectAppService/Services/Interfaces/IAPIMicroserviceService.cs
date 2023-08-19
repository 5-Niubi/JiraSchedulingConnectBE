namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IAPIMicroserviceService
    {
        public void SetDomain(string configObj);
        public Task<HttpResponseMessage> Get(string url);
        public Task<HttpResponseMessage> Post(string url, dynamic contentObject);
    }
}
