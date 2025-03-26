using Microsoft.AspNetCore.Mvc;
using UnknownCampServer.Core.Repositories;
using UnknownCampServer.Core.Services;
using UnknownCampServer.Infrastructure.Config;
using UnknownCampServer.Infrastructure.Data;
using UnknownCampServer.Infrastructure.Repositories;
using UnknownCampServer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection("AppConfig"));

builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddTransient<IPasswordService, PasswordService>();

builder.Services.AddTransient<IEmailRepository, EmailRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IAccountService, AccountService>();

builder.Services.AddTransient<ITreasureRepository, TreasureRepository>();
builder.Services.AddTransient<ITreasureService, TreasureService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
