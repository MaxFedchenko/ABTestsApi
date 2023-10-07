using ABTestsApi.Common.ActionFilters;
using ABTestsApi.Models.DTOs;
using ABTestsApi.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ABTestsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "DeviceTokenAuthenticationScheme")]
    public class ExperimentsController : ControllerBase
    {
        private readonly IBtnColorProviderService _btnColorProviderService;
        private readonly IPriceProviderService _priceProviderService;

        public ExperimentsController(IBtnColorProviderService btnColorProviderService, IPriceProviderService priceProviderService) 
        {
            _btnColorProviderService = btnColorProviderService;
            _priceProviderService = priceProviderService;
        }

        [HttpGet("button-color")]
        [TypeFilter(typeof(ExperimentFilterAttribute), Arguments = new[] { "button-color" })]
        public IActionResult GetButtonColor() 
        {
            return Ok(new KeyValueDTO 
            { 
                Key = "button_color", 
                Value = _btnColorProviderService.Get() 
            });
        }
        [HttpGet("price")]
        [TypeFilter(typeof(ExperimentFilterAttribute), Arguments = new[] { "price" })]
        public IActionResult GetPrice()
        {
            return Ok(new KeyValueDTO
            {
                Key = "price",
                Value = _priceProviderService.Get().ToString()
            }); ;
        }
    }
}
