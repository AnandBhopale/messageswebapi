

using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;


namespace messageswebapi
{
    

    public enum messagestatus
    {
        [Description("Idle")]
        idle,
        [Description("Running")]
        running,
        [Description("Finished")]
        finished,
        [Description("Errored")]
        errored
    }



public class Socketmesssages
    {
        public string id { get; set; }
        public string machine_id { get; set; }
        public DateTime timestamp { get; set; }

        [EnumDataType(typeof(messagestatus))]
        [JsonConverter(typeof(StringEnumConverter))]
        public messagestatus status { get; set; }
        
    }
}
