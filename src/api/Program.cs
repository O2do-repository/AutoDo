var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello AutoDo, Eric is the best !");

app.MapGet("/generate-error", () =>
{
    throw new Exception("Error : test log !");
});
app.Run();
