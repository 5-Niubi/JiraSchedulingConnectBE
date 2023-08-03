namespace ModelLibrary.DTOs
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FeatureAttribute : Attribute
    {
        public FeatureAttribute(string FunctionName)
        {
            FunctionName = FunctionName;
        }
    }
}

