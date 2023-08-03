using System.Net;
using System.Text.Json;
using FlexibleDataApi;
using FlexibleDataApi.Handlers;
using FlexibleDataApi.Models;
using FlexibleDataApi.Models.DTO;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("config.json", optional: true, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add db context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configuring validator
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure Media R
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddScoped<INotificationHandler<ProcessAndStoreEvent>, PostProcessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var fdApp = app.MapGroup("/flexibledata");

fdApp.MapPost("/create", async (IMediator mediator, DataContext context, IValidator<FlexibleDataCreateDto> _validation, [FromBody] FlexibleDataCreateDto flexibleDataCreateDto) =>
{
    var response = new ApiResponse();

    // Validation
    var validationResult = await _validation.ValidateAsync(flexibleDataCreateDto);
    if (!validationResult.IsValid)
    {
        response.ISuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }
    string jsonData = JsonSerializer.Serialize(flexibleDataCreateDto.Data);

    var flexibleData = new FlexibleData
    {
        Data = jsonData
    };

    var entityEntry = context.FlexibleDatas.Add(flexibleData);

    await context.SaveChangesAsync();

    var createdId = entityEntry.Entity.Id;

    response.Result = flexibleData;
    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    await mediator.Publish(new ProcessAndStoreEvent { Data = flexibleDataCreateDto.Data });


    return Results.CreatedAtRoute("GetFlexibleData", new { id = createdId }, response);
})
.WithName("CreateFlexibleData");

fdApp.MapGet("/get", async (DataContext context, [FromQuery] int? id) =>
{
    var response = new ApiResponse();
    if (id.HasValue)
    {

        var flexibleData = await context.FlexibleDatas.FindAsync(id);

        if (flexibleData == null)
        {
            response.ISuccess = false;
            response.StatusCode = HttpStatusCode.NotFound;
            response.ErrorMessages.Add($"No FlexibleData found with id: {id}");
            return Results.NotFound(response);
        }

        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(flexibleData.Data);
        List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>
        {
            data
        };

        response.Result = dataList;
    }
    else
    {
        var allFlexibleData = context.FlexibleDatas.ToList();

        var dataObjects = allFlexibleData
            .Select(flexibleData => new { id = flexibleData.Id, Data = JsonSerializer.Deserialize<Dictionary<string, object>>(flexibleData.Data) })
            .ToList();

        response.Result = dataObjects;
    }

    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetFlexibleData");

fdApp.MapGet("/get/{key}", (DataContext context, string key) =>
{
    var response = new ApiResponse();

    var keyData = context.Statistics.FirstOrDefault(s => s.Key == key);
    if (keyData == null)
    {
        response.ISuccess = false;
        response.StatusCode = HttpStatusCode.NotFound;
        response.ErrorMessages.Add($"No Statistics found with key: {key}");
        return Results.NotFound(response);
    }

    response.Result = keyData;
    response.ISuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
})
.WithName("GetKeyCount");


app.Run();