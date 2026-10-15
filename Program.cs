using System.Text;
using Datos.Data;
using Datos.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Negocio.Interfaces;
using Negocio.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Cadena de conexión a PostgreSQL + Naming Convention para snake_case
builder.Services.AddDbContext<TrotamundosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// 2. Configuración de CORS (Permite conexiones desde React Native Web)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Autenticación JWT (los tokens los emite AuthService con la misma configuración)
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Secret.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// 4. Controladores
builder.Services.AddControllers();

// 5. Repositorios (Capa de Datos)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPublicacionRepository, PublicacionRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IMapaRepository, MapaRepository>();
builder.Services.AddScoped<IInvitacionAdminRepository, InvitacionAdminRepository>();
builder.Services.AddScoped<ICorreoBloqueadoRepository, CorreoBloqueadoRepository>();

// 6. Servicios de Negocio (Capa de Negocio)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPublicacionService, PublicacionService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IMapaService, MapaService>();
builder.Services.AddScoped<IInvitacionAdminService, InvitacionAdminService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();

// Limpieza automática de reportes resueltos y alertas leídas (24 h)
builder.Services.AddHostedService<Trotamundos.Services.LimpiezaReportesService>();

// 7. Servicios Técnicos (Implementaciones en Capa de Datos)
builder.Services.AddScoped<IEmailService, Datos.Services.EmailService>();
builder.Services.AddScoped<IFileStorageService, Datos.Services.FileStorageService>();
// El catálogo mundial de ciudades se lee una vez y queda en memoria
builder.Services.AddSingleton<ICatalogoCiudades, Datos.Services.CatalogoCiudadesService>();
// Búsqueda de agencias en la web. Se hace desde el servidor para no exponer
// claves en la app; Nominatim exige identificar la aplicación con User-Agent.
builder.Services.AddMemoryCache();
// Búsqueda de alojamientos, restaurantes y agencias sobre los puntos de interés
// de OpenStreetMap (Overpass). Se cachea por ciudad para no saturar el servicio.
builder.Services.AddHttpClient<IBusquedaLugares, Datos.Services.BusquedaLugaresService>(cliente =>
{
    cliente.Timeout = TimeSpan.FromSeconds(45);
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Trotamundos/1.0 (app de viajeros)");
    cliente.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es");
});

builder.Services.AddHttpClient<IBusquedaAgencias, Datos.Services.BusquedaAgenciasService>(cliente =>
{
    cliente.Timeout = TimeSpan.FromSeconds(12);
    cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Trotamundos/1.0 (app de viajeros)");
    cliente.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es");
});

// 8. Swagger / OpenAPI (con soporte para probar endpoints protegidos con Bearer)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Pegar el token JWT devuelto por /api/auth/login"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Crea el primer superadministrador (SuperAdmin:Email / SuperAdmin:Password) si todavía no existe ninguno
try
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ISuperAdminService>().AsegurarSuperAdminInicialAsync();
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "No se pudo asegurar el superadministrador inicial (¿faltan aplicar migraciones?).");
}

// Configuración del Middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⚠️ Usar CORS ANTES de Autorización y Controladores
app.UseCors("AllowAll");

// ❌ Desactivado para desarrollo local por HTTP (evita la redirección a HTTPS y el bloqueo de red)
// app.UseHttpsRedirection();

// Sirve las imágenes subidas desde wwwroot/uploads
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
