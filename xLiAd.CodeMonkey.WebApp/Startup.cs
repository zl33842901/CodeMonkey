using xLiAd.CodeMonkey.Entities;
using xLiAd.CodeMonkey.Entities.Dtos;
using xLiAd.CodeMonkey.Services.Assets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using xLiAd.DapperEx.Repository;
using xLiAd.DiagnosticLogCenter.Agent;
using xLiAd.Gaia.Core;
using xLiAd.MinioEx.AspNetCore;

namespace xLiAd.CodeMonkey.WebApp
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
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 5000; // 5000 items max
                options.ValueLengthLimit = 1024 * 1024 * 100; // 100MB max len form data
            });
            services.AddHttpClient();//不使用 HttpClientFactory 可以删掉这句
            services.AddHttpContextAccessor();//不需要 HttpContext 可以删掉这句
            var conf = Configuration.Get<ConfigEntity>();
            services.Configure<ConfigEntity>(Configuration);
            services.AddScoped<IConfigEntity>(x => x.GetService<IOptionsSnapshot<ConfigEntity>>().Value);
            services.AddScoped<IDbConnection>(x => new SqlConnection(x.GetService<IConfigEntity>().SqlConnectionString));
            services.AddScoped<IConnectionHolder>(x => { var conn = x.GetService<IDbConnection>(); return new ConnectionHolder(conn); });
            services.AddDiagnosticLog(x =>
            {
                x.CollectServerAddress = conf.ExceptionLogCenterUrl;
                x.ClientName = conf.ExceptionLogCenterClient;
                x.EnvName = conf.ExceptionLogCenterEnv;
                x.ForbbidenPath = new string[]
                {
                    "/css/(.*)","/js/(.*)","/images/(.*)"
                };
            });
            //不需要跨域的话，就不用下边这句
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.SetIsOriginAllowed(x =>
                    {
                        return x.EndsWith(".xliad.com") || x.Contains(".xliad.com:");
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod().AllowCredentials();
                });
            });
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
            //services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(opt =>
            //{
            //    opt.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
            //    opt.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            //    opt.JsonSerializerOptions.PropertyNamingPolicy = null;//不加这句是 camle 加这句是 pascal
            //});
            services.AddControllersWithViews().AddRazorRuntimeCompilation().AddNewtonsoftJson(opt =>
            {
                var resolver = opt.SerializerSettings.ContractResolver;
                if (resolver != null)
                {
                    var res = resolver as DefaultContractResolver;
                    res.NamingStrategy = null;  // <<!-- this removes the camelcasing
                }
                opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddGaiaWithSqlServer();
            services.AddMinioEx((sp, options) =>
            {
                var conf = sp.GetService<IConfigEntity>();
                options.MinioUrl = conf.MinioUrl;
                options.BucketName = conf.MinioBucketName;
                options.AccessKey = conf.MinioAccessKey;
                options.SecretKey = conf.MinioSecretKey;
                options.ImageProxyUrl = conf.MinioImageProxyUrl;
                options.DirectoryPolicy = xLiAd.MinioEx.DirectoryPolicy.ByMonth;
            });
        }
        public void ConfigureContainer(IServiceCollection services)
        {
            var repoTypes = typeof(Infrastructure.AuthRoleRepository).Assembly.GetTypes()
                .Where(x => x.Name.EndsWith("repository", StringComparison.OrdinalIgnoreCase) ||
                x.Name.EndsWith("service", StringComparison.OrdinalIgnoreCase));
            var servTypes = typeof(Services.AuthRoleService).Assembly.GetTypes()
                .Where(x => x.Name.EndsWith("service", StringComparison.OrdinalIgnoreCase) ||
                x.Name.EndsWith("serviceproxy", StringComparison.OrdinalIgnoreCase));
            foreach (var repoType in repoTypes)
            {
                if (repoType.IsInterface)
                    continue;
                var inter = repoType.GetInterfaces().Where(x => !x.FullName.StartsWith("xLiAd.DapperEx")).FirstOrDefault();
                if (inter != null)
                    services.AddScoped(inter, repoType);
            }
            foreach (var servType in servTypes)
            {
                if (servType.IsInterface)
                    continue;
                if (servType.Name.EndsWith("serviceproxy", StringComparison.OrdinalIgnoreCase))
                    continue;
                var inter = servType.GetInterfaces().FirstOrDefault();
                var proxy = servTypes.Where(x => x.Name == servType.Name + "Proxy").FirstOrDefault();
                if (inter != null)
                {
                    if (proxy == null)
                        services.AddScoped(inter, servType);
                    else
                    {
                        services.AddScoped(inter, proxy);
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors("default");//不需要跨域的话，就不用这句
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
