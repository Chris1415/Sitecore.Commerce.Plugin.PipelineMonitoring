using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    public class TriggerStackedPipelineMonitoringLog : CommercePipeline<NodeContext, NodeContext>, ITriggerStackedPipelineMonitoringLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProfileDefinitionPipeline" /> class.
        /// </summary>
        /// <param name="configuration">The definition.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public TriggerStackedPipelineMonitoringLog(IPipelineConfiguration<ITriggerStackedPipelineMonitoringLog> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}
