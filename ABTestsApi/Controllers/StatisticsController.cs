using ABTestsApi.Models.DTOs;
using ABTestsApi.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABTestsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IExperimentService _experimentService;

        public StatisticsController(IExperimentService experimentService) 
        {
            _experimentService = experimentService;
        }

        private static OptionStatisticsDTO Map(OptionStatistics optStat) =>
            new OptionStatisticsDTO
            {
                Value = optStat.Value,
                Chance = optStat.Chance,
                Devices = optStat.Devices
            };
        private static ExperimentStatisticsDTO Map(ExperimentStatistics exptStat) =>
             new ExperimentStatisticsDTO
             {
                 Name = exptStat.Name,
                 TotalDevices = exptStat.TotalDevices,
                 Options = exptStat.Options.Select(Map)
             };

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            var statistics = await _experimentService.GetStatistics();
            return Ok(statistics.Select(Map));
        }
    }
}
