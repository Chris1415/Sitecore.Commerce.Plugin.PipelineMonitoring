using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    [PipelineDisplayName("ConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Add complex types
            modelBuilder.AddEntityType(typeof(StackedConfiguredPipelineContainer));
            modelBuilder.AddEntityType(typeof(StackedConfiguredPipeline));
            modelBuilder.AddEntityType(typeof(StackedConfiguredBlock));
            modelBuilder.EntitySet<StackedConfiguredBlock>("StackedConfiguredBlock");
            modelBuilder.EntitySet<StackedConfiguredPipeline>("StackedConfiguredPipeline");
            modelBuilder.Function("GetStackedPipelines").ReturnsCollectionFromEntitySet<StackedConfiguredPipelineContainer>("Api");

            return Task.FromResult(modelBuilder);
        }
    }
}
