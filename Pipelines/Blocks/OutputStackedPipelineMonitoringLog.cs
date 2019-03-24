namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [PipelineDisplayName("PipelineMonitoring.OutputStackedPipelineMonitoringLog")]
    public class OutputStackedPipelineMonitoringLog : PipelineBlock<NodeContext, NodeContext, CommercePipelineExecutionContext>
    {
        private readonly GetStackedPipelineConfigurationCommand _getExtendedPipelineConfigurationCommand;

        public OutputStackedPipelineMonitoringLog(GetStackedPipelineConfigurationCommand getExtendedPipelineConfigurationCommand)
        {
            this._getExtendedPipelineConfigurationCommand = getExtendedPipelineConfigurationCommand;
        }

        public override async Task<NodeContext> Run(NodeContext nodeContext, CommercePipelineExecutionContext context)
        {
            OutputStackedPipelineMonitoringLog configurationBlock = this;
            Condition.Requires(nodeContext).IsNotNull(configurationBlock.Name + ": The NodeContext can not be null");
            var configuredPipelines = await configurationBlock._getExtendedPipelineConfigurationCommand.Process(context.CommerceContext);
            string path = nodeContext.LoggingPath + "PipelineMonitoring" + nodeContext.ContactId + ".log";
            try
            {
                if (!Directory.Exists(nodeContext.LoggingPath ?? string.Empty))
                    Directory.CreateDirectory(nodeContext.LoggingPath ?? string.Empty);
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    streamWriter.WriteLine("Pipeline Monitoring Log");
                    foreach (StackedConfiguredPipeline configuredPipeline in configuredPipelines.List)
                    {
                        streamWriter.WriteLine("-----------------------------------------------------------------");
                        string str = configuredPipeline.Namespace.Replace("Sitecore.Commerce.", string.Empty);
                        //streamWriter.WriteLine(configuredPipeline.Namespace ?? string.Empty);
                        streamWriter.WriteLine($"{(configuredPipeline.IsCustom ? "<CUSTOM>" : string.Empty)} {str}.{configuredPipeline.Name} ({configuredPipeline.Receives} => {configuredPipeline.Returns})");
                        foreach (StackedConfiguredBlock block in configuredPipeline.GetComponents<StackedConfiguredBlock>())
                        {
                            streamWriter.WriteLine($"   ------------------------------------------------------------");
                            str = block.Namespace.Replace("Sitecore.Commerce.", string.Empty);
                            streamWriter.WriteLine($"   {(configuredPipeline.IsCustom ? "<CUSTOM>" : string.Empty)} {str}.{block.Name}({block.Receives} => {block.Returns})");
                            CollectPipelineInformation(streamWriter, block, 2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(configurationBlock.Name + "-Error creating Node Configuration Log", ex);
            }
            return nodeContext;
        }

        private void CollectPipelineInformation(StreamWriter streamWriter, StackedConfiguredBlock block, int level)
        {
            string placeholder = new string(' ', level * 3);

            foreach (StackedConfiguredBlock childBlock in block.GetComponents<StackedConfiguredBlock>())
            {
                streamWriter.WriteLine($"{placeholder}------------------------------------------------------------");
                string str = block.Namespace.Replace("Sitecore.Commerce.", string.Empty);
                if (string.IsNullOrEmpty(childBlock.PropertyName))
                {
                    // Such case is given when block is BLOCK or PIPELINE
                    streamWriter.WriteLine($"{placeholder}{(childBlock.IsCustom ? "<CUSTOM>" : string.Empty)} {str}.{childBlock.Name}({childBlock.Receives} => {childBlock.Returns})");
                }
                else
                {
                    // Such case is given when block is COMMAND
                    streamWriter.WriteLine($"{placeholder}{(childBlock.IsCustom ? "<CUSTOM>" : string.Empty)} {str}.{childBlock.Name} {childBlock.PropertyName}");
                }

                CollectPipelineInformation(streamWriter, childBlock, level + 1);
            }
        }
    }
}