﻿namespace JiraSchedulingConnectAppService.DTOs.Authentication
{
    public class RequestExchangeAccessAndRefeshTokenDTO
    {
        public string? grant_type { get; set; }
        public string? client_id { get; set; }
        public string? client_secret { get; set; }
        public string? refresh_token { get; set; }    
    }
}
