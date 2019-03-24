using Sitecore.Commerce.Core;
using System.Collections.Generic;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    class PipelineMonitoringPolicy : Policy
    {
        public PipelineMonitoringPolicy()
        {
            this.CustomNamespaces = new List<string>();
        }

        public IList<string> CustomNamespaces {get;set;}
    }
}
