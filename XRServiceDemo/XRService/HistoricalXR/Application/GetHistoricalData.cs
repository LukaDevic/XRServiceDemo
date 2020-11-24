using MediatR;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XRService.HistoricalXR.Domain;

namespace XRService.HistoricalXR.Application
{
    public static class GetHistoricalData
    {
        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly HttpClient _client;
            private const string apiUrl = "https://api.exchangeratesapi.io/";

            public Handler(IHttpClientFactory clientFactory)
            {
                _client = clientFactory.CreateClient();
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var response = await GetHistoricFXFromApiAsync(query);

                if (!response.IsSuccessStatusCode) return Result.Empty;

                var orderedHistoricRates = await GetOrderedHistoricRatesAsync(response);

                if (orderedHistoricRates == null || orderedHistoricRates.Count == 0) return Result.Empty;

                var message = CreateExchangeRateInformation(orderedHistoricRates);

                return new Result(message);
            }

            private async Task<HttpResponseMessage> GetHistoricFXFromApiAsync(Query query)
            {
                var orderedDates = query.Dates.OrderBy(x => x.Date).ToList();

                var requestUrl =
                    $"{apiUrl}history?start_at={orderedDates.FirstOrDefault():yyyy-MM-dd}" +
                    $"&end_at={orderedDates.LastOrDefault():yyyy-MM-dd}" +
                    $"&base={query.BaseCurrency}" +
                    $"&symbols={query.TargetCurrency}";

                return await _client.GetAsync(requestUrl);
            }

            private async Task<List<Rate>> GetOrderedHistoricRatesAsync(HttpResponseMessage response)
            {
                var dataAsString = await response.Content.ReadAsStringAsync();
                dynamic content = JsonConvert.DeserializeObject<ExpandoObject>(dataAsString);
                var rates = content.rates;
                var historicRates = new List<Rate>();

                foreach (var rate in (IDictionary<string, object>) rates)
                {
                    foreach (var fxRateObj in (IDictionary<string, object>) rate.Value)
                    {
                        historicRates.Add(new Rate
                        {
                            Date = Convert.ToDateTime(rate.Key),
                            RateOnDate = Convert.ToDouble(fxRateObj.Value)
                        });
                    }
                }

                return historicRates.OrderBy(x => x.RateOnDate).ToList();
            }

            private ExchangeRateInformation CreateExchangeRateInformation(List<Rate> orderedHistoricRates)
            {
                var minimumRate = orderedHistoricRates.FirstOrDefault();
                var maximumRate = orderedHistoricRates.LastOrDefault();
                var averageRate = orderedHistoricRates.Select(x => x.RateOnDate).Average();

                var result = new ExchangeRateInformation
                {
                    MinimumRate = new RateInfo {Date = minimumRate.Date.ToString("yyyy-MM-dd"), Rate = minimumRate.RateOnDate},
                    MaximumRate = new RateInfo { Date = maximumRate.Date.ToString("yyyy-MM-dd"), Rate = maximumRate.RateOnDate },
                    AverageRate = averageRate
                };

                return result;
            }
        }

        public class Query : IRequest<Result>
        {
            public Query(List<DateTime> dates, string baseCurrency, string targetCurrency)
            {
                Dates = dates;
                BaseCurrency = baseCurrency;
                TargetCurrency = targetCurrency;
            }

            public List<DateTime> Dates { get; }

            public string BaseCurrency { get; }

            public string TargetCurrency { get; }
        }

        public class Result
        {
            public Result(ExchangeRateInformation exchangeRateInformation)
            {
                ExchangeRateInformation = exchangeRateInformation;
            }

            public ExchangeRateInformation ExchangeRateInformation { get; }

            public bool IsEmpty { get; private set; }

            public static Result Empty = new Result {IsEmpty = true};

            private Result() {}
        }
    }
}