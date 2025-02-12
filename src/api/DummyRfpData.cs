public class DummyRfpData
{
    public static List<ReceivedRequestInfoOutputModel> GetDummyRfpList()
    {
        return new List<ReceivedRequestInfoOutputModel>
        {
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "323e4567-e89b-12d3-a456-426614174222",
                Reference = "YPTO001379",
                Title = "Software Engineer",
                Description = "Develop and maintain software applications...",
                Context = "Agile development methodology...",
                Customer = new CompanyReferenceModel { Name = "YPT1" },
                AssignedPositions = 1,
                PublishedProposals = 3,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(5), // Encore valide
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Junior" },
                Department = new DepartmentReferenceModel { Name = "Software Development" },
                Location = new LocationReferenceModel { Name = "Gand" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Freelance" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "DEV" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/003", Reference = "RFP-003" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(10),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter3" },
                StartTime = "09:00",
                EndTime = "17:00",
                Pause = "1h",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeC" },
                IsProposalPublicationAllowed = true
            },
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "423e4567-e89b-12d3-a456-426614174333",
                Reference = "YPTO001380",
                Title = "Data Analyst",
                Description = "Analyze large datasets to extract insights...",
                Context = "Big data and machine learning projects...",
                Customer = new CompanyReferenceModel { Name = "YPT2" },
                AssignedPositions = 2,
                PublishedProposals = 4,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(7), // Encore valide
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Senior" },
                Department = new DepartmentReferenceModel { Name = "Data Science" },
                Location = new LocationReferenceModel { Name = "Liège" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Permanent" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "ANALYTICS" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/004", Reference = "RFP-004" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(15),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter4" },
                StartTime = "08:00",
                EndTime = "16:00",
                Pause = "30min",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeD" },
                IsProposalPublicationAllowed = false
            },
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "523e4567-e89b-12d3-a456-426614174444",
                Reference = "YPTO001381",
                Title = "Project Manager",
                Description = "Oversee project planning and execution...",
                Context = "Agile and waterfall methodologies...",
                Customer = new CompanyReferenceModel { Name = "YPT3" },
                AssignedPositions = 1,
                PublishedProposals = 2,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(6), // Encore valide
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Mid-Level" },
                Department = new DepartmentReferenceModel { Name = "Project Management" },
                Location = new LocationReferenceModel { Name = "Brussels" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Contract" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "PM" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/005", Reference = "RFP-005" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(20),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter5" },
                StartTime = "10:00",
                EndTime = "18:00",
                Pause = "45min",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeE" },
                IsProposalPublicationAllowed = true
            },
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "623e4567-e89b-12d3-a456-426614174555",
                Reference = "YPTO001382",
                Title = "Cybersecurity Specialist",
                Description = "Implement and manage security measures...",
                Context = "Cyber threat analysis and risk mitigation...",
                Customer = new CompanyReferenceModel { Name = "YPT4" },
                AssignedPositions = 3,
                PublishedProposals = 5,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(8), // Encore valide
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Expert" },
                Department = new DepartmentReferenceModel { Name = "Cybersecurity" },
                Location = new LocationReferenceModel { Name = "Antwerp" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Consulting" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "SECURITY" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/006", Reference = "RFP-006" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(25),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter6" },
                StartTime = "07:30",
                EndTime = "15:30",
                Pause = "1h",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeF" },
                IsProposalPublicationAllowed = false
            },
            // Nouveaux enregistrements avec dates expirées
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "723e4567-e89b-12d3-a456-426614174666",
                Reference = "YPTO001383",
                Title = "Business Analyst",
                Description = "Business analysis for enterprise projects...",
                Context = "Work closely with business stakeholders...",
                Customer = new CompanyReferenceModel { Name = "YPT5" },
                AssignedPositions = 2,
                PublishedProposals = 3,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(-3), // Expiré
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Mid-Level" },
                Department = new DepartmentReferenceModel { Name = "Business Analysis" },
                Location = new LocationReferenceModel { Name = "Ghent" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Permanent" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "ANALYST" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/007", Reference = "RFP-007" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(8),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter7" },
                StartTime = "09:00",
                EndTime = "17:00",
                Pause = "1h",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeG" },
                IsProposalPublicationAllowed = true
            },
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "823e4567-e89b-12d3-a456-426614174777",
                Reference = "YPTO001384",
                Title = "DevOps Engineer",
                Description = "Manage cloud infrastructure and CI/CD pipelines...",
                Context = "Infrastructure as code, automated deployments...",
                Customer = new CompanyReferenceModel { Name = "YPT6" },
                AssignedPositions = 2,
                PublishedProposals = 4,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(-5), // Expiré
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Senior" },
                Department = new DepartmentReferenceModel { Name = "Cloud Infrastructure" },
                Location = new LocationReferenceModel { Name = "Brussels" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Freelance" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "DEVOPS" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/008", Reference = "RFP-008" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(10),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter8" },
                StartTime = "08:00",
                EndTime = "16:00",
                Pause = "45min",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeH" },
                IsProposalPublicationAllowed = false
            },
            new ReceivedRequestInfoOutputModel
            {
                Uuid = "923e4567-e89b-12d3-a456-426614174888",
                Reference = "YPTO001385",
                Title = "Cloud Architect",
                Description = "Design scalable cloud-based solutions...",
                Context = "Cloud architecture and microservices...",
                Customer = new CompanyReferenceModel { Name = "YPT7" },
                AssignedPositions = 3,
                PublishedProposals = 6,
                ResponseDate = DateTimeOffset.UtcNow.AddDays(3), // valide
                ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Lead" },
                Department = new DepartmentReferenceModel { Name = "Cloud Solutions" },
                Location = new LocationReferenceModel { Name = "Antwerp" },
                AssignmentType = new AssignmentTypeReferenceModel { Code = "Contract" },
                AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "CLOUD" },
                ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/009", Reference = "RFP-009" },
                CreatedAt = DateTimeOffset.UtcNow,
                PreferredStartDate = DateTimeOffset.UtcNow.AddDays(15),
                CostCenter = new CostCenterReferenceModel { Name = "CostCenter9" },
                StartTime = "09:00",
                EndTime = "17:00",
                Pause = "1h",
                Type = new ReceivedRequestTypeReferenceModel { Code = "TypeI" },
                IsProposalPublicationAllowed = true
            }
        };
    }
}
