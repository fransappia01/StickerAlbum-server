using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StickerAlbum.Context;
using System;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AlbumDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "1099274674013-b4huu7p8trdbdquntm3vp2fddaqj30ne.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:3000", "http://192.168.182:3000");
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
