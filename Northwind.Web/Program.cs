var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region the HTTP pipeline and routes

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapGet("/", () => $"Environment is {app.Environment.EnvironmentName}");

#endregion

// Start the web server, host the website, and wait for requests.
app.Run(); // This is a thread-blocking call.
Console.WriteLine("This executes after the web server has stopped!");
