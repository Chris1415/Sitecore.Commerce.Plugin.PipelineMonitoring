namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Framework.Pipelines.Definitions;

    public class GetStackedPipelineConfigurationCommand : CommerceCommand
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISitecoreServicesConfiguration _serviceCollection;
        public GetStackedPipelineConfigurationCommand(IServiceProvider serviceProvider, ISitecoreServicesConfiguration services)
          : base(serviceProvider)
        {
            this._serviceProvider = serviceProvider;
            this._serviceCollection = services;
        }

        public Task<StackedConfiguredPipelineContainer> Process(CommerceContext commerceContext)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                List<Type> list = this._serviceCollection.Services
                    .Where((x => typeof(IPipeline).IsAssignableFrom(x.ServiceType)))
                    .Select((x => x.ServiceType))
                    .Where((p => p.IsInterface))
                    .ToList();

                StackedConfiguredPipelineContainer result = new StackedConfiguredPipelineContainer();
                foreach (Type type in list)
                {
                    IPipelineBlockDescriptor instance = Activator.CreateInstance(typeof(PipelineBlockDescriptor<>).MakeGenericType(type)) as IPipelineBlockDescriptor;
                    StackedConfiguredPipeline configuredPipeline = new StackedConfiguredPipeline
                    {
                        Namespace = instance?.Type.Namespace,
                        Name = instance?.Type.Name,
                        Receives = instance?.Receives.FullName,
                        Returns = instance?.Returns.FullName,
                        IsCustom = Assembly.GetAssembly(instance.Type).GetCustomAttribute<CustomAssemblyAttribute>() != null
                    };

                    foreach (var block in GetBlocksOfPipeline(type, commerceContext))
                    {
                        configuredPipeline.Components.Add(block);
                    }


                    result.List.Add(configuredPipeline);

                }
                return Task.FromResult(result);
            }
        }

        private List<StackedConfiguredBlock> GetBlocksOfPipeline(Type type, CommerceContext context)
        {
            List<StackedConfiguredBlock> blocks = new List<StackedConfiguredBlock>();
            try
            {
                object requiredService = this._serviceProvider.GetRequiredService(typeof(IPipelineConfiguration<>).MakeGenericType(type));
                FieldInfo field = requiredService.GetType().GetField("_collected", BindingFlags.Instance | BindingFlags.NonPublic);
                object obj = field?.GetValue(requiredService);
                if (obj is List<IBuildablePipelineBlockDefinition> pipelineBlockDefinitionList)
                {
                    foreach (IBuildablePipelineBlockDefinition pipelineBlockDefinition in pipelineBlockDefinitionList)
                    {
                        IPipelineBlockDescriptor pipelineBlockDescriptor = pipelineBlockDefinition.Describe();
                        StackedConfiguredBlock configuredBlock = new StackedConfiguredBlock
                        {
                            Namespace = pipelineBlockDescriptor.Type.Namespace,
                            Name = pipelineBlockDescriptor.Type.Name,
                            Receives = pipelineBlockDescriptor.Receives.FullName,
                            Returns = pipelineBlockDescriptor.Returns.FullName,
                            IsCustom = Assembly.GetAssembly(pipelineBlockDescriptor.Type).GetCustomAttribute<CustomAssemblyAttribute>() != null
                        };
                        foreach (var child in GetChildrenBlocks(pipelineBlockDescriptor.Type, context))
                        {
                            configuredBlock.ChildComponents.Add(child);
                        }

                        blocks.Add(configuredBlock);
                    }
                }

            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.Message);
            }

            return blocks;
        }

        private List<StackedConfiguredBlock> GetChildrenBlocks(Type parentBlock, CommerceContext context)
        {
            List<StackedConfiguredBlock> childBlocks = new List<StackedConfiguredBlock>();
            IEnumerable<PropertyFieldInfo> proertiesAndFields = parentBlock?.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Select(element => new PropertyFieldInfo() { Name = element.Name, GivenType = element.PropertyType })
                .Union(parentBlock?.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Select(element => new PropertyFieldInfo() { Name = element.Name, GivenType = element.FieldType }));

            foreach (var property in proertiesAndFields)
            {
                if (typeof(IPipeline).IsAssignableFrom(property.GivenType))
                {
                    IPipelineBlockDescriptor instance = Activator.CreateInstance(typeof(PipelineBlockDescriptor<>).MakeGenericType(property.GivenType)) as IPipelineBlockDescriptor;
                    childBlocks.Add(new StackedConfiguredBlock()
                    {
                        PropertyName = string.Empty,
                        Namespace = instance?.Type.Namespace,
                        Name = instance?.Type.Name,
                        Receives = instance.Receives.FullName,
                        Returns = instance.Returns.FullName,
                        IsCustom = Assembly.GetAssembly(instance.Type).GetCustomAttribute<CustomAssemblyAttribute>() != null
                    });
                    continue;
                }

                if (typeof(IPipelineBlock).IsAssignableFrom(property.GivenType))
                {
                    //
                }

                if (property.GivenType.IsSubclassOf(typeof(CommerceCommand)))
                {
                    var stackedConfigurationBlock = new StackedConfiguredBlock()
                    {
                        PropertyName = property.Name,
                        Namespace = property.GivenType.Namespace,
                        Name = property.GivenType.Name,
                        Receives = string.Empty,
                        Returns = string.Empty,
                        IsCustom = Assembly.GetAssembly(property.GivenType).GetCustomAttribute<CustomAssemblyAttribute>() != null
                    };
                    foreach (var child in GetChildrenBlocks(property.GivenType, context))
                    {
                        stackedConfigurationBlock.ChildComponents.Add(child);
                    }
                    childBlocks.Add(stackedConfigurationBlock);
                }
            }

            return childBlocks;
        }
    }
}