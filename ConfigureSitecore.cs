using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .AddPipeline<ITriggerStackedPipelineMonitoringLog, TriggerStackedPipelineMonitoringLog>(c => c
                    .Add<OutputStackedPipelineMonitoringLog>())
               .ConfigurePipeline<IStartNodePipeline>(configure => configure
                    .Add<OutputStackedPipelineMonitoringLog>().After<StartNodeLogConfigurationBlock>())
               .ConfigurePipeline<IConfigureServiceApiPipeline>(c => c
                    .Add<PipelineMonitoring.ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}