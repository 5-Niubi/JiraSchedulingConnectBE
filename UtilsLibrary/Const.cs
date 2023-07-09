namespace JiraSchedulingConnectAppService.Common
{
    public class Const
    {
        public static class Claims
        {
            public static string CLOUD_ID = "cloud_id";
            public static string ACCOUNT_ID = "account_id";
        }

        public static class PAGING
        {
            public static int NUMBER_RECORD_PAGE = 10;
        }

        public static class RESOURCE_TYPE
        {
            public static string WORKFORCE = "workforce";
            public static string EQUIPMENT = "equipment";
        }

        public static class MESSAGE
        {
            public const string SUCCESS = "Success!!!";
            public const string PROJECT_NAME_EXIST = "Project name already exists.";
            public const string JIRA_API_ERROR = "Error when make a request to JIRA";
            public const string NOTFOUND_SCHEDULE = "Not found schedule";
        }
    }
}
