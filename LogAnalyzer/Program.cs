using LogAnalyzerLibrary.Helpers.ConfigurationSettings;
using LogAnalyzerLibrary.Helpers.ConfigurationSettings.ConfigManager;
using LogAnalyzerLibrary.Helpers.Filters;
using LogAnalyzerLibrary.Helpers.FolderHelper;
using LogAnalyzerLibrary.Interfaces;
using LogAnalyzerLibrary.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Assign the IConfiguration object to the ConfigurationSettingsHelper
ConfigurationSettingsHelper.Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ILogCountService, LogCountRepository>();
builder.Services.AddScoped<ILogManagementService, LogManagementRepository>();
builder.Services.AddScoped<ILogSearchService, LogSearchRepository>();
builder.Services.AddScoped<ILogUploadService, LogUploadRepository>();

builder.Services.AddSwaggerGen(options =>
{
    // Retrieve the XML file name and path
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Include the XML documentation in Swagger
    options.IncludeXmlComments(xmlPath);
    options.SchemaFilter<EnumSchemaFilter>();
});

// Grant folder permissions using the static method
FolderSearchHelper.GrantFullControlToFolder(ConfigSettings.ApplicationSetting.PathForPermission);


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
