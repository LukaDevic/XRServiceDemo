using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XRService.HistoricalXR.Application;
using XRService.HistoricalXR.Domain;

namespace XRServiceWeb.Features.HistoricalXR
{
    [Route("api/historicalxr")]
    [ApiController]
    public class HistoricalXRController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HistoricalXRController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ExchangeRateInformation>> GetHistoricalExchangeRates([FromQuery] ListParameters parameters)
        {
            var result = await _mediator.Send(new GetHistoricalData.Query(parameters.Dates, parameters.BaseCurrency, parameters.TargetCurrency));

            if (result.IsEmpty)
            {
                return NotFound();
            }

            return result.ExchangeRateInformation;
        }
    }
}