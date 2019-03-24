using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    public class StackedConfiguredBlock : Component
    {
        public string PropertyName { get; set; }

        public bool IsCustom { get; set; }

        public string Namespace { get; set; }

        public string Receives { get; set; }

        public string Returns { get; set; }
    }
}
