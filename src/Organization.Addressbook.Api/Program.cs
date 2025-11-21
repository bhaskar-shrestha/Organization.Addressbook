using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Organization.Addressbook.Api.Data;

var builder = WebApplication.CreateBuilder(args);


// Always use SQLite
builder.Services.AddDbContext<AddressBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=addressbook.db"));

builder.Services.AddControllers();
// Add Swagger/OpenAPI only as a development aid
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<Organization.Addressbook.Api.Services.IOrganizationService, Organization.Addressbook.Api.Services.OrganizationService>();
builder.Services.AddScoped<Organization.Addressbook.Api.Services.IBranchService, Organization.Addressbook.Api.Services.BranchService>();
builder.Services.AddScoped<Organization.Addressbook.Api.Services.IPersonService, Organization.Addressbook.Api.Services.PersonService>();
builder.Services.AddScoped<Organization.Addressbook.Api.Services.IMappingService, Organization.Addressbook.Api.Services.Mapping.MappingService>();

var app = builder.Build();

// Configure middleware and top-level route registrations
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Enable Swagger UI in development only
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Use top-level route registration for controllers
app.MapControllers();

app.Run();

// Expose Program class for WebApplicationFactory integration tests
public partial class Program { }
