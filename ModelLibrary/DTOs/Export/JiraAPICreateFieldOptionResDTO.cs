namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateFieldOptionResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Option
        {
            public string Id
            {
                get; set;
            }
            public string Value
            {
                get; set;
            }
            public bool? Disabled
            {
                get; set;
            }
            public string OptionId
            {
                get; set;
            }
        }

        public class Root
        {
            public List<Option> Options
            {
                get; set;
            }
        }
    }
}
