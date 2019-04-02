using System;
using System.IO;
using System.Reflection;

using AutoMapper;
using AutoMapper.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;

using UserManager.Api.Config;
using UserManager.Api.Impl;
using UserManager.Domain.Requests;
using UserManager.Domain.Responses;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace UserManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // AutomMapper configuration
            var mapperConfig = new MapperConfigurationExpression();

            mapperConfig.CreateMap<LoginRequest, IdentityUser>().ForMember(x => x.UserName, opt => opt.MapFrom(y => y.Email));
            mapperConfig.CreateMap<IdentityUser, LoginResponse>();

            Mapper.Initialize(mapperConfig);
            services.AddSingleton(Mapper.Instance);

            // Email configuration
            services.AddTransient<IEmailSender, SendGridEmailSender>();
            services.Configure<SendGridSettings>(Configuration);

            // Database configuration
            var databaseSettings = new DatabaseSettings();
            Configuration.Bind("Database", databaseSettings);
            services.AddSingleton(databaseSettings);

            services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(databaseSettings.ConnectionString, optionsBuilder => optionsBuilder.MigrationsAssembly("UserManager.Api")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(90);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.SignIn.RequireConfirmedEmail = true;

                options.User.RequireUniqueEmail = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info
                {
                    Title = "User management REST services",
                    Version = "v1",
                    Description = "A REST service with commands to manage users and roles.",
                    Contact = new Contact
                    {
                        Name = "Chad Wescott",
                        Email = "chadwescott@gmail.com"
                    }
                });
                x.EnableAnnotations();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IdentityDbContext dbContext)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management REST Services");
                x.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseErrorHandlingMiddleware();
                dbContext.Database.Migrate();
                //builder.AddUserSecrets<Startup>();
            }
            else
            {
                app.UseHsts();
                app.UseErrorHandlingMiddleware();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
