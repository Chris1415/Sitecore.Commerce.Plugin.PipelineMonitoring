using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    public class StackedConfiguredPipeline : CommerceEntity
    {
        public string Namespace { get; set; }

        public string Receives { get; set; }

        public string Returns { get; set; }

        public string Comment { get; set; }

        public bool IsCustom { get; set; }
    }
}
