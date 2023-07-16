namespace JiraSchedulingConnectAppService.Common
{
    public class Const
    {
        public static int THREAD_ID_COUNT_START = 1;
        public static int THREAD_ID_LENGTH = 10;
        public static string SPACE = " ";

        public static class Claims
        {
            public static string CLOUD_ID = "cloud_id";
            public static string ACCOUNT_ID = "account_id";
        }

        public static class DELETE_STATE
        {
            public const bool DELETE = true;
            public const bool NOT_DELETE = false;
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
            public const string MICROSERVICE_API_ERROR = "Error when make a request to other service";
            public const string NOTFOUND_SCHEDULE = "Not found schedule";
        }

        public static class THREAD_STATUS
        {
            public const string SUCCESS = "success";
            public const string RUNNING = "running";
            public const string ERROR = "error";
        }
    }
}
