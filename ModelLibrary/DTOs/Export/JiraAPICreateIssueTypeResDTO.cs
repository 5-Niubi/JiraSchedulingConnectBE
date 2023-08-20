namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateIssueTypeResDTO
    {
        public int? AvatarId
        {
            get; set;
        }
        public string? Description
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
        public string? IconUrl
        {
            get; set;
        }
        public string? Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public ScopeDTO? Scope
        {
            get; set;
        }
        public string? Self
        {
            get; set;
        }

        public class AvatarUrlsDTO
        {
        }

        public class ProjectDTO
        {
            public AvatarUrlsDTO? AvatarUrls
            {
                get; set;
            }
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
            public ProjectCategory? ProjectCategory
            {
                get; set;
            }
            public string? ProjectTypeKey
            {
                get; set;
            }
            public string? Self
            {
                get; set;
            }
            public bool? Simplified
            {
                get; set;
            }
        }

        public class ProjectCategory
        {
        }



        public class ScopeDTO
        {
            public ProjectDTO? Project
            {
                get; set;
            }
            public string? Type
            {
                get; set;
            }
        }
    }
}
