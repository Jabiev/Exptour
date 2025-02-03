var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerDocument(configure =>
{
    configure.PostProcess = (doc =>
    {
        doc.Info.Title = "Explore Tour Guide";
        doc.Info.Version = "1.0.0";
        doc.Info.Description = "Booking all you need | Cars, Guides, Hotels, Ready Packages and everything you need";
        doc.Info.Contact = new NSwag.OpenApiContact()
        {
            Name = "E x p t o u r",
            Url = "https://www.youtube.com/@iamjabiev",
            Email = "jabieviam@gmail.com"
        };
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", handler =>
{
    handler.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
