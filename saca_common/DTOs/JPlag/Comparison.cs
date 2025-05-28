using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.JPlag
{
    public class Comparison
    {
        [JsonPropertyName("first_submission")]
        public string FirstSubmission { get; set; } = null!;

        [JsonPropertyName("second_submission")]
        public string SecondSubmission { get; set; } = null!;

        [JsonPropertyName("similarities")]
        public Similarity Similarities { get; set; } = null!;
    }

    public class Similarity
    {
        [JsonPropertyName("AVG")]
        public double Avg { get; set; }

        [JsonPropertyName("MAX")]
        public double Max { get; set; }
    }

    public class OverviewData
    {
        [JsonPropertyName("top_comparisons")]
        public List<Comparison> TopComparisons { get; set; } = new List<Comparison>();
    }
}
