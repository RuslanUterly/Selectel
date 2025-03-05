
using Amazon.S3;
using Selectel.Services;

namespace Selectel;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddCors();

        builder.Services.AddControllers();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddSingleton<IAmazonS3>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var config = new AmazonS3Config
            {
                ServiceURL = configuration["S3Storage:ServiceURL"],
                ForcePathStyle = true,
                AuthenticationRegion = configuration["S3Storage:Region"]
            };

            return new AmazonS3Client(
                configuration["S3Storage:AccessKey"],
                configuration["S3Storage:SecretKey"],
                config
            );
        });

        builder.Services.AddScoped<SelectelStorageService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors(options => options
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );


        app.MapControllers();

        app.Run();
    }
}
