
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB_viz_DynamoDB
{
    public class MovieModel 
    {
        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("title")]
        public string Title{ get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
        public MovieModel(string title, int year)
        {
            this.Title = title;
            this.Year = year;
        }
        public MovieModel()
        {

        }
        [JsonProperty("info")]
        public   MovieInfo MovieInfo { get; set; }

        internal string PrintInfo()
        {
            if(this.MovieInfo!=null)
            return            string.Format("\nMovie with title:{1}\n Year: {2}, Actors: {3}\n Directors:{4}\n Rating:{5}\n", this.Id, this.Title, this.Year, String.Join(",",this.MovieInfo.Actors), this.MovieInfo, this.MovieInfo.Rating);
            else
                return string.Format("\nMovie with  title:{0}\n Year: {1}\n",  this.Title, this.Year);
        }
    }
}
