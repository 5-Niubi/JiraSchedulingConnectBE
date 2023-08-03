namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIErrorResDTO
    {
        public class Errors
        {
        }

        public class Root
        {
            public List<string> errorMessages
            {
                get; set;
            }
            public Errors errors
            {
                get; set;
            }
        }

    }
}
