using System;
using System.Collections.Generic;
using XRService.HistoricalXR.Validation;

namespace XRService.HistoricalXR.Domain
{
    [AllParametersAreRequired]
    public class ListParameters
    {
        public List<DateTime> Dates { get; set; }

        public string BaseCurrency { get; set; }

        public string TargetCurrency { get; set; }
    }
}
