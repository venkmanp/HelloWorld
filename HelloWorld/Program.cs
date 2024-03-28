using Serilog;
using Microsoft.AspNetCore.StaticFiles;
using HelloWorld.Services;
using HelloWorld.Contexts;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HelloWorld
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                .WriteTo.File("logs/shoplog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddProblemDetails(op =>
            {
                op.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions.Add("Somedata", "This is some data");
                    ctx.ProblemDetails.Extensions.Add("MachineName", Environment.MachineName);
                };

            });

            builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
#if DEBUG
            builder.Services.AddTransient<IMailService, LocalMailService>();
#else
            builder.Services.AddTransient<IMailService, RealMailService>();
#endif

            builder.Services.AddDbContext<MainContext>(
                opt => opt.UseSqlite(builder.Configuration["ConnectionStrings:Main"])
                );
            
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IUserRepository, UserRespository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new()
                    {
                        //Here we specify what we want to validate in the token: the issuer, the algorithm, the audienceetc.
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        //Here we specify what to validate against: what the issuer should match, etc.
                        ValidIssuer = builder.Configuration["Authentication:Issuer"],
                        ValidAudience = builder.Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]))
                    };
                });

            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("IsAdmin", p=> {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("auth", "10");
                });
            });
            
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            //app.Run(async (ctx) =>
            //{
            //    await ctx.Response.WriteAsync("Hello from Simon");
            //});

            app.Run();
        }
    }
}
