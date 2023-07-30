using System.Net;
using FlexibleDataApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var fdApp = app.MapGroup("/flexibledata");

fdApp.MapPost("/create", () =>
{
    var response = new ApiResponse();
    response.Result = new { id = 1 };
    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.CreatedAtRoute("GetFlexibleData", new { id = 1 }, response);
})
.WithName("CreateFlexibleData");

fdApp.MapGet("/get/{id:int}", (int id) =>
{
    var response = new ApiResponse();
    response.Result = new { id };
    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetFlexibleData");

fdApp.MapGet("/get/{key:alpha}", (string key) =>
{
    var response = new ApiResponse();
    response.Result = new { key };
    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetKeyCount");


app.Run();