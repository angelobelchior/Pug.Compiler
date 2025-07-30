namespace Pug.Compiler.Editor.Endpoints;

[ExcludeFromCodeCoverage]
internal static class Samples
{
    public static void AddSamplesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/samples", () =>
        {
            var samples = Models.Sample.CreateSamples();
            return Results.Ok(samples);
        });
    }
}