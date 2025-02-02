﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RzrSite.API.Filters;
using RzrSite.API.Middleware;
using RzrSite.API.Services;
using RzrSite.DAL;
using RzrSite.DAL.Repositories;
using RzrSite.DAL.Repositories.Interfaces;
using RzrSite.Models.Responses.DbFile;

namespace RzrSite.API
{
	public class Startup
	{
		public IConfiguration Configuration;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAutoMapper(typeof(RzrSiteDbContext), typeof(AddedDbFile));

			services.AddCors();

			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders =
						ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			});

			services.AddControllers(options =>
				{
					options.Filters.Add(new EntityNotFoundFilter());
					options.Filters.Add(new InconsistentStructureFilter());
				});

			services.AddScoped<ICategoryRepo, CategoryRepo>();
			services.AddScoped<IProductLineRepo, ProductLineRepo>();
			services.AddScoped<IDbFileRepo, DbFileRepo>();
			services.AddScoped<IProductRepo, ProductRepo>();
			services.AddScoped<IFeatureRepo, FeatureRepo>();
			services.AddScoped<IAdvantageRepo, AdvantageRepo>();
			services.AddScoped<IImageRepo, ImageRepo>();
			services.AddScoped<IVideoRepo, VideoRepo>();
			services.AddScoped<IFeatureTypeRepo, FeatureTypeRepo>();

			services.AddSwaggerDocument();

			services.Configure<EmailServiceConfig>(Configuration.GetSection("EmailService"));
			services.AddTransient<IEmailService, EmailService>();

			services.AddDbContext<RzrSiteDbContext>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}
			app.UseCors(
				options => options.WithOrigins("http://rzrsite-backend.ru", "http://rzrsite.cashtusk.ru")
													.AllowAnyMethod()
													.AllowAnyHeader()
													.AllowCredentials()
			);

			app.UseOpenApi();
			app.UseSwaggerUi3();

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
