using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SFServer.Contexts;
using SFServer.Security;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using SFServer.Middleware;
using SFServer.Services;
using SFServer.Filters;
using SFServer.Scheduler;
using SFServer.Scheduler.Scheduling;
using SFServer.Scheduler.Tasks;
using SFServer.Discord;
using DSharpPlus;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Hosting;
using SFServer.Utility;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using System.Net.Sockets;
using SFServer.Models.Configs;
using SFServer.Services.Giphy;
using SFServer.Services.iAP.Apple;
using SFServer.Services.iAP.Google;
using SFServer.Configs;
//using Microsoft.OpenApi.Models;

namespace SFServer
{
    public class Startup
    {
        ILogger logger;
        ILoggerFactory loggerFactory;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;

            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger("Startup");
        }

        public static IConfiguration Configuration { get; set; }
        public static IWebHostEnvironment WebHostEnvironment { get; set; }
        public static IServiceCollection Services { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;

            services.AddAuthentication("Bearer")
                .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>("Bearer", null);

            services.AddMvc().AddNewtonsoftJson().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            //Load configs
            services.Configure<VersionConfig>(Configuration);
            services.Configure<RewardConfig>(Configuration);
            services.Configure<PriceConfig>(Configuration);
            services.Configure<GameConfig>(Configuration);
            services.Configure<SlotMachineConfig>(Configuration);
            services.Configure<CardsConfig>(Configuration);

            ConfigureDatabase(services, Configuration, WebHostEnvironment);

            services.AddTransient<TokenService>();
            services.AddTransient<TransactionService>();
            services.AddTransient<NotificationService>();
            services.AddTransient<DailyChallengeService>();
            services.AddTransient<NewLevelOfTheWeekService>();
            services.AddTransient<RewardService>();
            services.AddTransient<BoostService>();
            services.AddTransient<LevelStatsService>();
            services.AddTransient<GameStatisticsService>();
            services.AddTransient<EndlessService>();
            services.AddTransient<SlotMachineService>();
            services.AddTransient<CardsService>();

            services.AddSingleton<ClientSecretAuthenticator>();
            services.AddSingleton(new RSAEncrypt());
            services.AddSingleton(new GiphyService(loggerFactory));
            services.AddSingleton(new AppleAppStoreService(loggerFactory));
            services.AddSingleton(new GooglePlayStoreService(loggerFactory));

            services.AddScoped<ClientVersionValidationAttribute>();
            services.AddScoped<ClientSecretValidationAttribute>();
            services.AddScoped<ModelValidationAttribute>();
            services.AddScoped<LegalVerificationAttribute>();

            services.AddSingleton<IScheduledTask, TaskTemplate>();
            services.AddSingleton<IScheduledTask, DailyChallengeTask>();
            services.AddSingleton<IScheduledTask, SendDailyChallangeNotificationTask>();
            services.AddSingleton<IScheduledTask, NewLevelOfTheWeekTask>();
            services.AddSingleton<IScheduledTask, RecentPlaysTask>();
            services.AddSingleton<IScheduledTask, TrendingLevelsTask>();
            services.AddSingleton<IScheduledTask, EraseInactiveTask>();
            services.AddSingleton<IScheduledTask, SyncGameStatisticsTask>();
            services.AddSingleton<IScheduledTask, UpdateDiscordGameStatistics>();
            services.AddSingleton<IScheduledTask, DiscordInviteTask>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
            */
            ConfigureDiscord(services);

            services.AddScheduler((sender, args) =>
            {
                Console.WriteLine(args.Exception.Message);
                args.SetObserved();
            }, WebHostEnvironment);
        }

        public void ConfigureDiscord(IServiceCollection services)
        {
            DiscordClient discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration["DISCORD_TOKEN"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = DSharpPlus.LogLevel.Error
            });

            services.AddSingleton<DiscordClient>(discordClient);
            VersionConfig version = services.BuildServiceProvider().GetService<IOptions<VersionConfig>>().Value;
            DailyChallengeService dailyChallengeService = services.BuildServiceProvider().GetService<DailyChallengeService>();
            services.AddSingleton<PepeBot>(new PepeBot(services.BuildServiceProvider().GetService<PostgreSqlContext>(), discordClient, version, dailyChallengeService, services.BuildServiceProvider().GetService<LevelStatsService>()));
            services.AddTransient<DiscordService>();
        }

        private void ConfigureDatabase(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            // https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-dotnetcore-sqldb

            string connectionString = null;

            if (webHostEnvironment.IsDevelopment())
            {
                //Use docker-machine env to get the correct ip for local development on windows! On mac use localhost ip
                string mssqlIP = LocalIPAddress().ToString();
                connectionString = $"Host={mssqlIP};Port=5432;Username=postgres;Password=[REDACTED];Database=Safari;";
            }
            else
            {
                connectionString = Configuration["CONNECTION_STRING"];
            }

            services.AddDbContext<PostgreSqlContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            services.BuildServiceProvider().GetService<PostgreSqlContext>().Database.Migrate();
        }

        private void ConfigureStaticFiles(IApplicationBuilder app)
        {
            app.UseFileServer();
            StaticFileOptions option = new StaticFileOptions();
            FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider ??
            new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(".unityweb", "application/octet-stream");
            option.ContentTypeProvider = contentTypeProvider;
            app.UseStaticFiles(option);
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            ConfigureStaticFiles(app);
            //app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            /*
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            */

            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
       

            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            //this code is called when the application stops
            DiscordClient discordClient = Services.BuildServiceProvider().GetService<DiscordClient>();

            discordClient.DisconnectAsync().Wait();
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
