using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


// ==================================================
// Controllers
// ==================================================

builder.Services.AddControllers();


// ==================================================
// CORS
// ==================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


// ==================================================
// Authentication
// ==================================================

builder.Services
    .AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyCookieAuth";

        options.Cookie.HttpOnly = true;

        options.Cookie.SameSite =
            SameSiteMode.None;

        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        // 30 seconds currently
        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(30);

        options.SlidingExpiration = false;

        options.Events =
            new Microsoft
                .AspNetCore
                .Authentication
                .Cookies
                .CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;

                    return Task.CompletedTask;
                },

                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;

                    return Task.CompletedTask;
                }
            };
    });


// ==================================================
// Authorization
// ==================================================

builder.Services.AddAuthorization(options =>
{
    // Admin
    options.AddPolicy(
        "AdminOnly",
        policy =>
            policy.RequireClaim(
                "admin",
                "true")
    );


    // HR
    options.AddPolicy(
        "HrOnly",
        policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(
                    "admin",
                    "true")
                ||
                context.User.HasClaim(
                    "department",
                    "hr")
            )
    );


    // HR Manager
    options.AddPolicy(
        "HrManagerOnly",
        policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(
                    "admin",
                    "true")
                ||
                (
                    context.User.HasClaim(
                        "department",
                        "hr")
                    &&
                    context.User.HasClaim(
                        "role",
                        "manager")
                )
            )
    );
});


// ==================================================
// Swagger
// ==================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// ==================================================
// Build
// ==================================================

var app = builder.Build();


// ==================================================
// Swagger UI
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ==================================================
// Middleware
// ==================================================

app.UseHttpsRedirection();

app.UseCors("Angular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();