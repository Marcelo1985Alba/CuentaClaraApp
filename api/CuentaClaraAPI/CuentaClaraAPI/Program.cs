using CuentaClara.API.Extensions;
using CuentaClara.Application.Interfaces;
using CuentaClara.Application.Services;
using CuentaClara.Domain.Interfaces;
using CuentaClara.Infrastructure.Data;
using CuentaClara.Infrastructure.Models;
using CuentaClara.Infrastructure.Repositories;
using CuentaClara.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configurar conexión a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar HttpContextAccessor para acceder al contexto desde servicios
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// Configurar Identity
builder.Services.AddIdentity<AppUser, AppUserRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Configurar la política de bloqueo de cuentas
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

    //options.SignIn.RequireConfirmedAccount = false;
    //options.SignIn.RequireConfirmedEmail = false;
    //options.SignIn.RequireConfirmedPhoneNumber = false;
    //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    

    options.User.RequireUniqueEmail = true;

})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsync("Unauthorized");
            return Task.CompletedTask;
        },

        OnMessageReceived = context =>
        {
            // Buscar el token en las cookies en lugar de los headers
            var token = context.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"Cookie encontrada: {!string.IsNullOrEmpty(token)}");
                context.Token = token;
            }
            else
            {
                Console.WriteLine("No se encontró la cookie AuthToken");
            }

            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
//.AddCookie(options => {
//    options.Cookie.Name = "AuthToken";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cámbialo a Always si usas HTTPS
//    options.ExpireTimeSpan = TimeSpan.FromHours(4);
//    options.SlidingExpiration = true;
//    options.LoginPath = "/api/users/login";
//    options.LogoutPath = "/api/users/logout";
//    options.AccessDeniedPath = "/api/users/access-denied";

//    options.Events = new CookieAuthenticationEvents
//    {
//        // Este evento se dispara cuando se valida la cookie
//        OnValidatePrincipal = async context =>
//        {
//            // Este evento se dispara cuando se valida la cookie
//            var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtGenerator>();
//            try
//            {
//                // Extraer el token JWT del valor de la cookie
//                var cookieValue = context.Request.Cookies["Auth.Token"];
//                if (string.IsNullOrEmpty(cookieValue))
//                {
//                    context.RejectPrincipal();
//                    return;
//                }

//                // Validar el token y obtener los claims
//                var principal = jwtService.ValidateToken(cookieValue);

//                if (principal == null)
//                {
//                    context.RejectPrincipal();
//                    return;
//                }

//                // Reemplazar el principal de la cookie con el principal del JWT
//                context.Principal = principal;
//            }
//            catch
//            {
//                // En caso de error, rechazar la autenticación
//                context.RejectPrincipal();
//            }
//        }
//    };
//});


// Inyección de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();


builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Personaliza la información de tu API aquí
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cuenta Clara API",
        Version = "v1",
        Description = "Cuenta Clara API"
    });

    // Añadir soporte para cookies en Swagger
    c.AddSecurityDefinition("CookieAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
        Name = "AuthToken",
        Description = "Cookie de autenticación via api/Users/Login",
        Scheme = "Cookie"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "CookieAuth"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")  // Dirección de tu aplicación Angular
              .AllowCredentials() // Permitir el uso de credenciales (cookies)
              .AllowAnyHeader()  // Permite cualquier encabezado
              .AllowAnyMethod(); // Permite cualquier método HTTP (GET, POST, PUT, DELETE, etc.)
    });
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.Name = "AuthToken";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cámbialo a Always si usas HTTPS
//});


var app = builder.Build();

app.UseErrorHandler();

// Habilitar CORS para la API
app.UseCors("AllowAngularApp");  // Usa la política definida previamente
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Configura la interfaz de usuario de Swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cuenta Clara API V1");
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
        c.RoutePrefix = "swagger";
        c.ConfigObject.AdditionalItems.Add("withCredentials", true);
        // Agregar script personalizado para incluir cookies en las peticiones
        c.HeadContent = @"
    <script>
      window.onload = function() {
        if (!window.SwaggerUIStandalonePreset) {
          setTimeout(() => window.onload(), 100);
          return;
        }
        
        const oldFetch = window.fetch;
        window.fetch = function(url, options) {
          options = options || {};
          options.credentials = 'include';
          return oldFetch(url, options);
        };
      }
    </script>";
    });
}



app.Run();
