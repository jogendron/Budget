var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) => {
    var path = context.Request.Path.Value;
    var physicalPath = Path.Join(app.Environment.WebRootPath, path);

    if (string.IsNullOrEmpty(path) || path == "/" || (path.StartsWith("/en") && ! File.Exists(physicalPath))) 
        context.Request.Path = "/en/index.html";
    else if (path == "/fr" || (path.StartsWith("/fr") && ! File.Exists(physicalPath)))
        context.Request.Path = "/fr/index.html";
    else if (path.StartsWith("/en/assets"))
        context.Request.Path = $"/en/assets/{path}";
    else if (path.StartsWith("/fr/assets"))
        context.Request.Path = $"/fr/assets/{path}";
    else if (path.StartsWith("/assets"))
        context.Request.Path = $"/en{path}";

    await next(context);
});

app.UseStaticFiles();

app.Run();
