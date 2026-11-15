using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Chave JWT não configurada.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("A chave JWT deve possuir pelo menos 32 bytes.");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("Issuer JWT não configurado.");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Audience JWT não configurado.");

var jwtExpirationMinutes = builder.Configuration.GetValue<int>("Jwt:ExpirationMinutes");

if (jwtExpirationMinutes <= 0)
    throw new InvalidOperationException("A validade do token JWT deve ser maior que zero.");

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

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IDoacaoService, DoacaoService>();
builder.Services.AddScoped<IEmprestimoService, EmprestimoService>();
builder.Services.AddHostedService<EmprestimoExpiracaoService>();
builder.Services.AddScoped<IParceriaService, ParceriaService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

// Serviços da aplicação
builder.Services.AddControllers();

// Configuração de respostas padronizadas para erros
builder.Services.AddProblemDetails();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        var origensPermitidas = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (origensPermitidas == null || origensPermitidas.Length == 0)
            throw new InvalidOperationException("Nenhuma origem foi configurada para o CORS.");

        policy.WithOrigins(origensPermitidas).AllowAnyHeader().AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Description = "Informe o token JWT."
        });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Configuração OpenAPI nativa
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();

    await usuarioService.CriarAdministradorInicialAsync();
}

// Tratamento global de exceções
app.UseExceptionHandler();

// Configuração do ambiente
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interface visual do Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();