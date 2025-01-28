var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Eric is the best !");

app.MapGet("/generate-error", () =>
{
    try
    {
        throw new Exception("Ceci est une exception de test !");
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
});
app.Run();
