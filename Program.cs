using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskFlow.Data;
using TaskFlow.Models;
using TaskFlow.Services;

var builder = WebApplication.CreateBuilder(args); // Sets up the app: config, DI container, logging
builder.Services.AddEndpointsApiExplorer(); // Discovers our API endpoints so Swagger can document them
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Registers our DB context, tells EF Core to use SQL Server with our connection string
builder.Services.AddControllers(); // Enables controller support (routes HTTP requests to Controller classes)
builder.Services.AddScoped<ITaskItemService, TaskItemService>(); // Registers TaskItemService in DI
builder.Services.AddScoped<ITokenService, TokenService>(); // Registers TokenService in DI — generates JWTs on register/login
// Generates the OpenAPI/Swagger JSON doc, configured with JWT Bearer support for the Authorize button
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // Tells Swagger UI how to accept a token (shows the Authorize button)
    {
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement // Applies the Bearer scheme to every endpoint in Swagger UI, so the lock icon shows up on each one
    { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
});
// Registers Identity: password hashing, user creation, uniqueness checks — all handled internally
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDbContext>();
// Registers JWT Bearer authentication — tells the app how to validate incoming tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!))
    };
});

var app = builder.Build(); // Builds the actual app from all the configured services above
app.UseExceptionHandler(errorApp => // Catches unhandled exceptions from any endpoint
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500; // Unhandled errors return HTTP 500
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"Something went wrong.\"}"); // Generic message — hides internal details/stack trace from the client for security
    });
});
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); } // Serves swagger.json and the visual UI page, dev only
app.UseHttpsRedirection(); // Forces HTTP requests to redirect to HTTPS
app.UseAuthentication(); // Identifies who the user is (validates the JWT token) — must come before UseAuthorization
app.UseAuthorization(); // Decides whether the identified user is allowed to access the requested endpoint
app.MapControllers(); // Maps controller routes to actual HTTP endpoints
app.Run(); // Starts listening for requests (blocks here, keeps app alive)