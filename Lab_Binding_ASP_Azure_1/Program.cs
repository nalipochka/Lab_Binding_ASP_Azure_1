using Azure.Storage.Blobs;
using Lab_Binding_ASP_Azure_1.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
string dbConnStr = builder.Configuration.GetConnectionString("photoDb");
builder.Services.AddDbContext<PhotoContext>(opt=>opt.UseSqlServer(dbConnStr));

builder.Services.AddTransient<BlobServiceClient>(factory =>
{
    string connStr = builder.Configuration.GetSection("AZURE_STORAGE_CONNECTION_STRING").Value;
    return new BlobServiceClient(connStr);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();