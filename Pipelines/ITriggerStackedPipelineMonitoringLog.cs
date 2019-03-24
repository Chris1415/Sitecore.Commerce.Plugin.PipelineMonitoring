using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    [PipelineDisplayName("PipelineMonitoring.Pipeline.ITriggerPipelineMonitoringLog")]
    public interface ITriggerStackedPipelineMonitoringLog : IPipeline<NodeContext, NodeContext, CommercePipelineExecutionContext>
    {
    }
}
