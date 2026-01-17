using DocumentToPdf.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDocumentToPdfConverter, AsposeDocumentToPdfConverter>();
// Since the converter is stateless and thread-safe, Singleton lifetime is the most efficient and correct choice.

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
