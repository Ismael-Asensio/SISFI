namespace SistemaFinanciero.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Prefer ASPNETCORE_URLS if set; otherwise bind to http://localhost:5272 for local development
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5272";
            builder.WebHost.UseUrls(urls);

            // A. Configurar CORS (Para conectar con React)
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });

            // B. Agregar Controladores y forzar serialización JSON en camelCase
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    // Allow case-insensitive matching when deserializing incoming JSON
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // C. Configurar Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registrar el servicio de cálculos explícitamente.
            // Usar una registración directa evita problemas de resolución por reflexión.
            builder.Services.AddScoped<SistemaFinanciero.API.Services.CalculosService>();


            var app = builder.Build();

            // Log the URLs we're listening on (helpful for diagnosis)
            Console.WriteLine($"Starting SistemaFinanciero.API on: {urls}");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}