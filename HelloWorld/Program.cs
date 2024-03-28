using Serilog;
using Microsoft.AspNetCore.StaticFiles;
using HelloWorld.Services;
using HelloWorld.Contexts;
using Microsoft.EntityFrameworkCore;
using HelloWorld.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using System.Reflection;
using Asp.Versioning.ApiExplorer;

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

            //Add this for API versioning support in swagger

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
                o.AddPolicy("IsAdmin", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("auth", "10");
                });
            });

            builder.Services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            })
            .AddMvc()
            //Add this for Api versioning support in swagger!
            .AddApiExplorer(o =>
            {
                o.SubstituteApiVersionInUrl = true;
            });

            var apiProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            builder.Services.AddSwaggerGen(o =>
            {
                var file = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var path = Path.Combine(AppContext.BaseDirectory, file);

                o.IncludeXmlComments(path);

                foreach (var desc in apiProvider.ApiVersionDescriptions) //build a swagger doc for each api version
                {
                    o.SwaggerDoc("MyAPI", new()
                    {
                        Title = "Ths is my cool API",
                        Version = desc.ApiVersion.ToString()
                    });
                }

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
