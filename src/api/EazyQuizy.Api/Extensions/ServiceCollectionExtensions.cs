using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace EazyQuizy.Api.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDefaultCorsPolicy(this IServiceCollection services, string name)
	{
		services.AddCors(options =>
		{
			options.AddPolicy(name, config => config
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowAnyOrigin()
				.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "X-Grpc-Web", "User-Agent")
				.SetPreflightMaxAge(TimeSpan.FromHours(1))
				.Build());
		});
		return services;
	}
	public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
	{
		var authority = configuration["Authority"] ?? throw new ArgumentNullException("Authority");
		services.AddAuthorization();
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configure =>
			{
				configure.TokenValidationParameters = new() { ValidateAudience = false, ValidateIssuer = false };
				configure.RequireHttpsMetadata = false;
				configure.BackchannelHttpHandler = new HttpClientHandler()
				{
					ServerCertificateCustomValidationCallback =
						delegate { return true; }
				};
				configure.MapInboundClaims = false;
				configure.Authority = authority;
			});
		return services;
	}
	public static IServiceCollection AddSwaggerGeneration(this IServiceCollection services, IConfiguration configuration)
	{
		var authority = configuration["Authority"] ?? throw new ArgumentNullException("Authority");
		var auth = new Uri($"{authority}/protocol/openid-connect/auth");
		var token = new Uri($"{authority}/protocol/openid-connect/token");
		services.AddGrpcSwagger();
		return services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "EazyQuizy API", Version = "v1" });

			c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
			{
				Type = SecuritySchemeType.OAuth2,
				Name = "Authorization",
				Flows = new OpenApiOAuthFlows
				{
					
					AuthorizationCode = new OpenApiOAuthFlow
					{
						AuthorizationUrl = auth,
						TokenUrl = token
					}
				},
				Description = "Open API security scheme",
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "oauth2"
						}
					},
					Array.Empty<string>()
				}
			});
		});
	}
}