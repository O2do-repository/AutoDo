using models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)  // Allow any origin
               .AllowAnyMethod()              // Allow any HTTP method
               .AllowAnyHeader()              // Allow any headers
               .AllowCredentials();           // Allow credentials
    });
});


var app = builder.Build();



// API /rfp avec données
app.MapGet("/rfp",  () =>
{
    var receivedDataList = new List<ReceivedRequestInfoOutputModel>
    {
        new ReceivedRequestInfoOutputModel
        {
            Uuid = "123e4567-e89b-12d3-a456-426614174000",
            Reference = "YPTO001377",
            Title = "Functional analyst YPR8916",
            Description = "Please upload the candidates on this platform...",
            Context = "Onderhandelingsprocedure...",
            Customer = new CompanyReferenceModel { Name = "YPT0" },
            AssignedPositions = 2,
            PublishedProposals = 5,
            ResponseDate = DateTimeOffset.Parse("2025-02-19T00:00:00Z"),
            ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Senior" },
            Department = new DepartmentReferenceModel { Name = "Transport Operations" },
            Location = new LocationReferenceModel { Name = "Bruxelles" },
            AssignmentType = new AssignmentTypeReferenceModel { Code = "Time & Material" },
            AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "PS" },
            ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/001", Reference = "RFP-001" },
            CreatedAt = DateTimeOffset.UtcNow,
            PreferredStartDate = DateTimeOffset.Parse("2025-03-03T00:00:00Z"),
            CostCenter = new CostCenterReferenceModel { Name = "CostCenter1" },
            StartTime = "09:00",
            EndTime = "17:00",
            Pause = "1h",
            Type = new ReceivedRequestTypeReferenceModel { Code = "TypeA" },
            IsProposalPublicationAllowed = true
        },
        new ReceivedRequestInfoOutputModel
        {
            Uuid = "223e4567-e89b-12d3-a456-426614174111",
            Reference = "YPTO001378",
            Title = "Project Manager",
            Description = "Manage projects with agile methodology...",
            Context = "Onderhandelingsprocedure...",
            Customer = new CompanyReferenceModel { Name = "YPT0" },
            AssignedPositions = 3,
            PublishedProposals = 4,
            ResponseDate = DateTimeOffset.Parse("2025-03-01T00:00:00Z"),
            ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Medior" },
            Department = new DepartmentReferenceModel { Name = "IT Operations" },
            Location = new LocationReferenceModel { Name = "Anvers" },
            AssignmentType = new AssignmentTypeReferenceModel { Code = "Fixed Price" },
            AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "IT" },
            ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/002", Reference = "RFP-002" },
            CreatedAt = DateTimeOffset.UtcNow,
            PreferredStartDate = DateTimeOffset.Parse("2025-04-01T00:00:00Z"),
            CostCenter = new CostCenterReferenceModel { Name = "CostCenter2" },
            StartTime = "08:30",
            EndTime = "16:30",
            Pause = "45min",
            Type = new ReceivedRequestTypeReferenceModel { Code = "TypeB" },
            IsProposalPublicationAllowed = false
        }
    };

    // Mapper la liste vers une liste de RFP
    var rfpList = receivedDataList.Select(data => data.ToRFP()).ToList();

    // Retourner la liste en JSON
    return Results.Json(rfpList);
});

// Route de test
app.MapGet("/", () => "Hello AutoDo,  Lucien and Eric are the best !");

app.UseCors("all");

app.Run();
