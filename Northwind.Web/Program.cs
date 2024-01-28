var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Start the web server, host the website, and wait for requests.
app.Run(); // This is a thread-blocking call.
Console.WriteLine("This executes after the web server has stopped!");
