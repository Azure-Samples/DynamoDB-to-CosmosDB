using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
    public class MovieInfo
    {
        [JsonProperty("rating")]
        public double Rating { get; set; }
        [JsonProperty("plot")]
        public string Plot { get; set; }
        [JsonProperty("directors")]
        public List<string> Directors { get; set; }
        [JsonProperty("actors")]
        public string[] Actors { get; set; }
    }
}
