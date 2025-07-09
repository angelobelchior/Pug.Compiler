using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Pug.Compiler.Editor.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();
app.MapRazorPages();

app.AddCompilerEndpoints();
app.AddSamplesEndpoints();

app.Run();