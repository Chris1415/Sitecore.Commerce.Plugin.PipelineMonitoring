using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Sitecore.Commerce.Core;
using System;
using System.Threading.Tasks;


namespace Sitecore.Commerce.Plugin.PipelineMonitoring
{
    public class ApiController : CommerceController
    {
        public ApiController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
          : base(serviceProvider, globalEnvironment)
        {
        }

        [HttpGet]
        [EnableQuery]
        [Route("api/GetStackedPipelines()")]
        public async Task<IActionResult> GetStackedPipelines()
        {
            var result = await this.Command<GetStackedPipelineConfigurationCommand>().Process(this.CurrentContext);
            return new ObjectResult(result.List);
        }
    }
}
