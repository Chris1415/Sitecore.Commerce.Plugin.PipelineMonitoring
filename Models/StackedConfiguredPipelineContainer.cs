using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    public class StackedConfiguredPipelineContainer : CommerceEntity
    {
        public StackedConfiguredPipelineContainer()
        {
            List = new List<StackedConfiguredPipeline>();
        }

        public List<StackedConfiguredPipeline> List { get; set; }
    }
}
