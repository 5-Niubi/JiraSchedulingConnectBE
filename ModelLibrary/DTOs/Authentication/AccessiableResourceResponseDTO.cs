namespace ModelLibrary.DTOs.Authentication
{
    public class AccessiableResourceResponseDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string[] scopes { get; set; }
        public string avatarUrl { get; set; }

        /*
          "id": "1324a887-45db-1bf4-1e99-ef0ff456d421",
    "name": "Site name",
    "url": "https://your-domain.atlassian.net",
    "scopes": [
      "write:jira-work",
      "read:jira-user",
      "manage:jira-configuration"
    ],
    "avatarUrl": "https:\/\/site-admin-avatar-cdn.prod.public.atl-paas.net\/avatars\/240\/flag.png"
         */
    }
}
