using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hackathon.Boilerlate.Api.Areas.Model
{
    public class APIModule
    {
        public String APIUrl { get; set; }
        public MultilistField SelectedHeaderValues { get; set; }
        public IEnumerable<InputParams> HeaderValues { get; set; }
        public MultilistField SelectedInputList { get; set; }
        public string InputParameters { get; set; }
        public string OutputParameters { get; set; }
    }

    public class InputParams
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}