using AlgorithmServiceServer.DTOs.AlgorithmController;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DBModels;
using System;
using System.Text.Json;

namespace JiraSchedulingConnectAppService.Services
{
    public class AlgorithmService : IAlgorithmService
    {
        private readonly IConfiguration config;
        private readonly HttpClient client;
        private readonly JiraDemoContext db;
        public AlgorithmService(IConfiguration configuration, JiraDemoContext db)
        {
            config = configuration;
            this.client = new HttpClient();
            this.db = db;
        }

        public void TestConverter(int projectId)
        {
            var baseUrl = config.GetValue<string[]>("Environment:AlgorithmServiceDomains");

            var contentObject = new InputToORDTO();
            

            var content = new StringContent(JsonSerializer.Serialize(contentObject));
            client.PostAsync(baseUrl[0] + "api/Algorithm/TestConverter", content);
        }

    }
}
