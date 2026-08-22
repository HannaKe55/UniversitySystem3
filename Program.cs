using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UniversitySystem3.Models;
using UniversitySystem3.Repositories;
using UniversitySystem3.Services;
using UniversitySystem3.Services.Auth;
using UniversitySystem3.Services.Class;
using UniversitySystem3.Services.Employees;
using UniversitySystem3.Services.Students;
using UniversitySystem3.Services.Teachers;
using UniversitySystem3.Services.Terms;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();

// Add the database
builder.Services.AddDbContext<UniversityDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UniversityDB")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(Options => {
    Options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header

        });
    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeacherClassService, TeacherClassService>();
builder.Services.AddScoped<ITeachersService, TeachersService>();
builder.Services.AddScoped<IStudentCourseManagementService, StudentCourseManagementService>();
builder.Services.AddScoped<IStudentCourseSelectionServicecs, StudentCourseSelectionService>();
builder.Services.AddScoped<IStudentService, StudentsService>();
builder.Services.AddScoped<IStudentInfoService, StudentInfoService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ITermService, TermService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
   
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

//Exception handeling middleware
app.UseMiddleware<UniversitySystem3.Middleware.ExceptionHandlingMiddleware>();


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

app.Run("https://0.0.0.0:7186");
