using models;

var builder = WebApplication.CreateBuilder(args);

// Ajouter Azure Monitor Logger
// builder.Logging.ClearProviders();
// builder.Logging.AddProvider(new AzureMonitorLoggerProvider());

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permet l'accès depuis n'importe quelle origine
              .AllowAnyMethod()  // Permet toutes les méthodes HTTP
              .AllowAnyHeader(); // Permet tous les en-têtes
    });
});

var app = builder.Build();

// Applique la politique CORS à toutes les routes
app.UseCors("AllowAll");

// API /rfp avec données
app.MapGet("/rfp", () =>
{
    var receivedData = new ReceivedRequestInfoOutputModel
    {
        Uuid = "123e4567-e89b-12d3-a456-426614174000",  // ID unique
        Reference = "YPTO001377",  // Référence spécifique de l'offre
        Title = "Functional analyst YPR8916",  // Titre du poste
        Description = "Please upload the candidates on this platform (Connecting Expertise) and also on our platform (Recruitee), by using this link: https://ypto.recruitee.com/o/ypr8916/c/new",  // Description du projet
        Context = "Onderhandelingsprocedure...",  // Contexte
        Customer = new CompanyReferenceModel { Name = "YPT0" },  // Nom du client
        AssignedPositions = 2,  // Nombre de personnes nécessaires
        PublishedProposals = 5,  // Nombre de propositions publiées
        ResponseDate = DateTimeOffset.Parse("2025-02-19T00:00:00Z"),  // Date limite de réponse
        ExternalStatus = new RequestExternalStatusReferenceModel { TranslatedName = "Senior" },  // Statut externe
        Department = new DepartmentReferenceModel { Name = "Transport Operations" },  // Département
        Location = new LocationReferenceModel { Name = "Bruxelles" },  // Lieu de travail
        AssignmentType = new AssignmentTypeReferenceModel { Code = "Time & Material" },  // Type de contrat
        AssignmentCategory = new AssignmentCategoryReferenceModel { Code = "PS" },  // Catégorie
        ProjectRequest = new ProjectRequestReferenceModel { ResourceUri = "https://example.com/rfp/001", Reference = "RFP-001" },  // Référence du projet
        CreatedAt = DateTimeOffset.UtcNow,  // Date de création
        PreferredStartDate = DateTimeOffset.Parse("2025-03-03T00:00:00Z"),  // Date de début
        CostCenter = new CostCenterReferenceModel { Name = "CostCenter1" },  // Centre de coût
        StartTime = "09:00",  // Heure de début
        EndTime = "17:00",  // Heure de fin
        Pause = "1h",  // Pause
        Type = new ReceivedRequestTypeReferenceModel { Code = "TypeA" },  // Type de la demande
        IsProposalPublicationAllowed = true  // Publication des propositions autorisée
    };

    // Mappez vers l'objet RFP
    RFP rfp = receivedData.ToRFP();

    // Renvoie le résultat en JSON
    return Results.Json(rfp);
});

// Route de test
app.MapGet("/", () => "Hello AutoDo, Lucien and Eric are the best  !");

app.Run();
