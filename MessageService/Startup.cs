using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using MessageService.Models;
using MessageService.Repository;
using MessageService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace MessageService
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
            MappingConfiguration.Global.Define<MessageMappings>();

            services.AddSingleton<ICluster>(p =>
            {
                var configCasandra = Configuration.GetSection("Casandra");
                var configCasandraUser = configCasandra.GetSection("User");
                var configCasandraNodes = configCasandra.GetSection("Nodes");
                var clusterBuilder = Cluster.Builder();
                if (configCasandraUser.Exists())
                {
                    clusterBuilder
                        .WithCredentials(configCasandraUser["username"], configCasandraUser["password"]);
                }
                foreach (var node in configCasandraNodes.GetChildren().Select(pp=>pp.Value))
                {
                    clusterBuilder.AddContactPoint(node);
                }
                return clusterBuilder.Build();
            });

         services.AddTransient<IMessageService, Services.MessageService>();

            services.AddTransient<IRepository<Message>,MessageRepository>();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            /*
             

            PoolingOptions poolingOptions = new PoolingOptions();
            poolingOptions.SetHeartBeatInterval(60000);

            ICluster cluster = Cluster.Builder()
                .AddContactPoints("127.0.0.1")
                .Build();
            var session = cluster.Connect("system");
            session.CreateKeyspaceIfNotExists("demo", new Dictionary<string, string>
            {
                {"class", "SimpleStrategy"},
                {"replication_factor", "2"}
            });
            session.ChangeKeyspace("demo");

            //  ISession session = cluster.Connect("demo");
            
             */
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
