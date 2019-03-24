# Sitecore.Commerce.Plugin.PipelineMonitoring
Sitecore Commerce Plugin for Extended Pipeline Monitoring

## What does the plugin do?

### Feature 1
This plugin extends the standard logging, like NodeConfiguration with an additional log file.
This new logfile contains information about defined pipelines and blocks, like the standard log.
In addition to the standard log this file also shows information of referenced Pipelines or Commands within Blocks recursively. This should help developers to better debug the application to understand which pipeline or command is called / used by other pipelines.

### Feature 2
Last feature of this plugin is, that it is able to also show, what code is "custom" one and what code is OOTB.
This also helps developers to see cahnges made to the standard pipelines and blocks. Custom blocks are marked with the prefix <CUSTOM>
To let the plugin know about such code, just use the assemblyattribute "CustomAssemblyAttribute" on your custom asseblies like this 
  
**[assembly: CustomAssembly()]**
  
## Usage  
This functionality is embedded on the one hand side into custom log file, but can also be grabbed by custom API function. This featues is BETA. The API can be used via the following URL:

**{{ServiceHost}}/api/GetStackedPipelines()?$expand=Components($expand=ChildComponents($expand=ChildComponents($expand=ChildComponents)))**

Because OData has some issue with recurive $expand operators, you have to expand to the level you want to see by your own by now.

## Example output of the new log file:

<pre>
  {CUSTOM} Plugin.Tickets.Pipelines.IActivateTicketPipeline (System.String => System.Boolean)
   ------------------------------------------------------------
   {CUSTOM} Plugin.Tickets.CheckTicketBlock(System.String => System.String)
      ------------------------------------------------------------
       Plugin.Tickets.FindEntityCommand _findEntityCommand
         ------------------------------------------------------------
          Core.Commands.IFindEntityPipeline(Sitecore.Commerce.Core.FindEntityArgument => Sitecore.Commerce.Core.CommerceEntity)
         ------------------------------------------------------------
          Core.Commands.ISecureEntityPipeline(Sitecore.Commerce.Core.CommerceEntity => Sitecore.Commerce.Core.CommerceEntity)
   ------------------------------------------------------------
   {CUSTOM} Plugin.Tickets.ActivateTicketBlock(System.String => System.String)
      ------------------------------------------------------------
       Plugin.Tickets.FindEntityCommand _findEntityCommand
         ------------------------------------------------------------
          Core.Commands.IFindEntityPipeline(Sitecore.Commerce.Core.FindEntityArgument => Sitecore.Commerce.Core.CommerceEntity)
         ------------------------------------------------------------
          Core.Commands.ISecureEntityPipeline(Sitecore.Commerce.Core.CommerceEntity => Sitecore.Commerce.Core.CommerceEntity)
      ------------------------------------------------------------
       Plugin.Tickets.PersistEntityCommand _persistEntityCommand
         ------------------------------------------------------------
          Core.Commands.IPersistEntityPipeline(Sitecore.Commerce.Core.PersistEntityArgument => Sitecore.Commerce.Core.PersistEntityArgument)
   ------------------------------------------------------------
   {CUSTOM} Plugin.Tickets.PersistEntityBlock(System.String => System.Boolean)
      ------------------------------------------------------------
       Plugin.Tickets.PersistEntityCommand _persistEntityCommand
         ------------------------------------------------------------
          Core.Commands.IPersistEntityPipeline(Sitecore.Commerce.Core.PersistEntityArgument => Sitecore.Commerce.Core.PersistEntityArgument)
     </pre>
