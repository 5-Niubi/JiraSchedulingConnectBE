namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIIssueTypeResDTO
    {
        public string? Self
        {
            get; set;
        }
        public string? Id
        {
            get; set;
        }
        public string? Description
        {
            get; set;
        }
        public string? IconUrl
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public bool? Subtask
        {
            get; set;
        }
        public int? AvatarId
        {
            get; set;
        }
        public string? EntityId
        {
            get; set;
        }
        public int? HierarchyLevel
        {
            get; set;
        }
        public ScopeDTO? Scope
        {
            get; set;
        }

        public class ProjectDTO
        {
            public string? Id
            {
                get; set;
            }
            public string? Key
            {
                get; set;
            }
            public string? Name
            {
                get; set;
            }
        }

        public class ScopeDTO
        {
            public string? Type
            {
                get; set;
            }
            public ProjectDTO? Project
            {
                get; set;
            }
        }
    }
}
