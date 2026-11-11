using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Joel_Norori_App_Ingenieria_de_Software.Data;
using Joel_Norori_App_Ingenieria_de_Software.Extension;
using Joel_Norori_App_Ingenieria_de_Software.Mapping;
using Joel_Norori_App_Ingenieria_de_Software.Middleware;
using Joel_Norori_App_Ingenieria_de_Software.Models.Settings;
using Joel_Norori_App_Ingenieria_de_Software.Repositories;
using Joel_Norori_App_Ingenieria_de_Software.Services;
using Joel_Norori_App_Ingenieria_de_Software.Authorization.JwtPermissionPolicies;
using Joel_Norori_App_Ingenieria_de_Software.Authorization;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

var jwtSettings =
    builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("No esta presente la configuracion del JWT");

builder.Services.AddAutoMapper(cfg=>cfg.AddMaps(typeof(UserProfile).Assembly));


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IUsuarioRespository, UsuarioRepository>();
builder.Services.AddScoped<IRoleRepository, RolRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

//=======================DBContext=========================
builder.Services.AddDbContext<AppDBContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoAuth")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token =
                        context.Request.Cookies["access_token"];
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/Login");
                }

                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/AccessDenied");
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                ClockSkew = TimeSpan.Zero
            };
    });
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin.panel", policy =>
        policy.RequireAssertion(context =>
            context.User.HasPermiso("rol.ver") ||
            context.User.HasPermiso("permiso.ver") ||
            context.User.HasPermiso("usuario.ver")));

    options.AddPolicy("permiso.actualizar", policy =>
        policy.RequireAssertion(context =>
            context.User.HasAnyPermiso("permiso.actualizar", "permiso.crear")));

    options.AddPolicy("permiso.borrar", policy =>
        policy.RequireAssertion(context =>
            context.User.HasAnyPermiso("permiso.borrar", "permiso.crear")));
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    await DbSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404 &&
        !response.HasStarted)
    {
        response.Redirect("/Home/NotFoundPage");
    }

    await Task.CompletedTask;
});
app.UseRouting();
app.UseSession();
app.UseMiddleware<JwtRefreshMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
