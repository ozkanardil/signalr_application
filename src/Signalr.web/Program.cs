using Signalr.web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseCors(builder => builder.WithOrigins("https://localhost:7008", "https://localhost").AllowAnyHeader().AllowAnyMethod());
app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:7290", "http://localhost:7290", "https://localhost:7008", "http://localhost:7008", "https://localhost", "http://localhost")
        .AllowAnyHeader()
        .WithMethods("GET", "POST")
        .AllowCredentials();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chathub");
});

app.UseAuthorization();

app.MapRazorPages();
//app.MapHub<ChatHub>("/chatHub");

app.Run();
