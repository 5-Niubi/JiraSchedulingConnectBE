namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IJiraBridgeAPIService
    {
        public Task<HttpResponseMessage> Get(string url);
        public Task<HttpResponseMessage> Post(string url, dynamic contentObject);
        public Task<HttpResponseMessage> Put(string url, dynamic contentObject);
        public Task<HttpResponseMessage> Delete(string url);

    }
}
