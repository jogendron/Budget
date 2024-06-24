var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) => {
    var path = context.Request.Path.Value;
    var physicalPath = Path.Join(app.Environment.WebRootPath, path);

    if (string.IsNullOrEmpty(path) || path == "/" || (path.StartsWith("/en") && ! File.Exists(physicalPath))) 
        context.Request.Path = "/en-US/index.html";
    else if (path == "/fr" || (path.StartsWith("/fr") && ! File.Exists(physicalPath)))
        context.Request.Path = "/fr-CA/index.html";
    else if (path.StartsWith("/assets"))
        context.Request.Path = $"/en-US{path}";

    await next(context);
});

app.UseStaticFiles();

app.Run();
