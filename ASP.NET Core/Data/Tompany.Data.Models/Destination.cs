using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class Destination : BaseModel<int>
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("population")]
        public int Population { get; set; }
    }
}
