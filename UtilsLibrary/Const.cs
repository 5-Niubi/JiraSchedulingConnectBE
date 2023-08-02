namespace UtilsLibrary
{
    public class Const
    {
        public static int THREAD_ID_COUNT_START = 1;
        public static int THREAD_ID_LENGTH = 10;
        public static string SPACE = " ";
        public static double DEFAULT_BASE_WORKING_HOUR = 8;

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
            public const int NUMBER_RECORD_PAGE = 10;
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
            public const string PROJECT_NAME_EMPTY = "Project Name empty!";
            public const string PROJECT_NAME_UPPER_1ST_CHAR = "Project Name must start by an uppercase letter";
            public const string PROJECT_WORKING_HOUR_ERR = "Invalid working hour";
            public const string PROJECT_BUDGET_ERR = "Invalid money value";
        }

        public static class THREAD_STATUS
        {
            public const string SUCCESS = "success";
            public const string RUNNING = "running";
            public const string ERROR = "error";
        }

        public static class  SUBSCRIPTION 
        {
            public const int PLAN_FREE = 1;
            public const int PLAN_PLUS = 2;
        }

        public static class ADMIN_SERVER
        {
            public const string USER = "user";
            public const int PLAN_FREE = 1;
            public const int PLAN_PREMIUM = 2;
        }
    }
}
