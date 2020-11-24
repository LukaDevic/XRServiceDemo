namespace XRService.HistoricalXR.Domain
{
    public class ExchangeRateInformation
    {
        public RateInfo MinimumRate { get; set; }

        public RateInfo MaximumRate { get; set; }

        public double AverageRate { get; set; }
    }
}
