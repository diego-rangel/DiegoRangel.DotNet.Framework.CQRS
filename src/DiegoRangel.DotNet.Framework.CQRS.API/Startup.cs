using System;
using DiegoRangel.DotNet.Framework.CQRS.API.Extensions;
using DiegoRangel.DotNet.Framework.CQRS.API.Filters;
using DiegoRangel.DotNet.Framework.CQRS.API.Temp;
using DiegoRangel.DotNet.Framework.CQRS.Infra.CrossCutting.IoC;
using DiegoRangel.DotNet.Framework.CQRS.Infra.CrossCutting.IoC.Extensions;
using DiegoRangel.DotNet.Framework.CQRS.Infra.CrossCutting.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DiegoRangel.DotNet.Framework.CQRS.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var assemblies = new[]
            {
                typeof(CommonMessages).Assembly,
                typeof(Domain.Core.Entities.Entity).Assembly,
                typeof(Infra.Data.EFCore.Setup.CustomDbContextOptionsBuilder).Assembly,
                typeof(Infra.Data.MongoDB.Context.MongoDbContext).Assembly,
            };

            services.AddControllers(options => { options.Filters.Add<ResponseValidationFilter>(); })
                .AddNewtonsoftJson();

            services.AddCulture("pt-BR");
            services.AddHealthChecks();
            services.AddCacheServices();
            services.AddCompression();
            services.AddIOServices();
            services.AddMediatr(assemblies)
                .AddRequestPerformanceBehavior()
                .AddUnhandledExceptionBehavior();

            services.AddSwaggerDocumentation(settings =>
            {
                settings.ApiTitle = "My test api Title";
                settings.ApiDescription = "My test api description";
                settings.ApiContactInfo = "no-reply@gmail.com";
                settings.SecureWithUseJwtAuth = false;
            });

            services.AddCommonMessages(messages =>
            {
                messages.NotFound = "Oops! Recurso n�o encontrado.";
                messages.InvalidOperation = "Oops! Opera��o inv�lida. Recarregue sua tela e tente novamente.";
                messages.UnhandledOperation = "Oops! N�o foi poss�vel processar a sua solicita��o no momento, tente novamente mais tarde.";
            });

            services.AddEmailServices(_env, settings =>
            {
                settings.Host = "smtp.gmail.com";
                settings.NoReplyMail = "no-reply@gmail.com";
                settings.UserName = "no-reply@gmail.com";
                settings.Password = "123456";
                settings.Port = 587;
                settings.EnableSsl = true;
                settings.UseDefaultCredentials = false;
            });

            services.AddAutoMapperWithSettings(settings =>
            {
                settings.UseStringTrimmingTransformers = true;
            }, assemblies);

            services.AddEfCoreServices();
            services.AddMongoDb(settings =>{});

            services.AddUserSignedInServices<TempUser, Guid,
                TempLoggedInUserProvider,
                TempLoggedInUserIdProvider,
                TempLoggedInUserIdentifierProvider>();

            Bootstrapper.RegisterServicesBasedOn<Guid>(services, assemblies);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseRouting();
            app.UseExceptionHandlers();
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            app.UseSwaggerDocumentation(settings =>
            {
                settings.ApiTitle = "My test api Title";
                settings.ApiDocExpansion = DocExpansion.Full;
            });
        }
    }
}
