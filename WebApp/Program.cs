var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
builder.Services.AddAuthentication().AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
    options.SlidingExpiration = false;

    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
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
builder.Services.AddAuthorization(options =>
{
    //AdminOnly policy requires the user to have a claim with type "admin" and value "true"
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin", "true"));

    //HR
    // Admin OR HR Department 
    options.AddPolicy("HrOnly", policy => policy.RequireAssertion(context =>
    context.User.HasClaim("admin", "true")
    ||
    context.User.HasClaim("department", "hr")
    ));


    // HR Manager
    // Admin OR (HR Department AND Role Manager)
    options.AddPolicy("HrManagerOnly", policy =>
    {
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
            ));
    });



});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
