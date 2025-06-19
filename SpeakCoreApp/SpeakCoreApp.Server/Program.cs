
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.EntityFrameworkCore;
using speakcore.API.Common;
using SpeakCoreApp.Server.Common.Interfaces;
using SpeakCoreApp.Server.Common.Services;

namespace SpeakCoreApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                       .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                       .CreateLogger();


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                           .AddJwtBearer(options => {
                               options.TokenValidationParameters = new TokenValidationParameters
                               {
                                   ValidateIssuer = true,
                                   ValidateAudience = true,
                                   ValidateLifetime = true,
                                   ValidateIssuerSigningKey = true,
                               };
                           });

            // Add SQLite
            builder.Services.AddDbContext<SpeakCoreDBContext>(options =>
                options.UseSqlite("Data Source=registrations.db"));

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<JwtService>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseCors("AllowAngular");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Map("/error", (HttpContext context) =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;

                Log.Error(exception, "Unhandled exception occurred");

                return Results.Problem(
                    title: "An unexpected error occurred!",
                    detail: exception?.Message,
                    statusCode: 500
                );
            });

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SpeakCoreDBContext>();
                context.Database.EnsureCreated();
            }


            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
