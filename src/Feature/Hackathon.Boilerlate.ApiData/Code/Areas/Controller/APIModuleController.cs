using Hackathon.Boilerlate.Api.Areas.Model;
using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Hackathon.Boilerlate.Api.Areas.Controller
{
    public class APIModuleController : SitecoreController
    {
        // GET: Controller/APIModule
        public override ActionResult Index()
        {
            var APIData = RenderingContext.Current.Rendering.Item;
            APIModule datasource = new APIModule();
            datasource.APIUrl = APIData.Fields["URL"].ToString();
            datasource.SelectedHeaderValues = APIData.Fields["Select Header Parameters"];
            datasource.HeaderValues = selectedMultilistValues(datasource.SelectedHeaderValues);
            datasource.SelectedInputList = APIData.Fields["Select Input Parameters"];
            datasource.InputParameters = selectedMultilistValues(datasource.SelectedInputList);

            var apiResult = InvokeJson<dynamic>(datasource);

            return View("~/Views/APIModule.cshtml", apiResult);
        }

        public IEnumerable<InputParams> selectedMultilistValues(MultilistField data)
        {
            IEnumerable<InputParams> selectedInputs = data.GetItems().Select(x => new InputParams
            {
                Key = x.Fields["Key"].ToString(),
                Value = x.Fields["Value"].ToString()
            });
            return selectedInputs;
        }

        public static T InvokeJson<T>(APIModule requestData) where T : class
        {
            try
            {
                Uri uri = new Uri(requestData.APIUrl);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.ContentType = "application/json";
                foreach(var headerInput in requestData.HeaderValues)
                {
                    request.Headers.Add(headerInput.Key, headerInput.Value);
                }
                
                string sb = JsonConvert.SerializeObject(requestData.InputParameters);
                byte[] data = Encoding.ASCII.GetBytes(sb);
                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    StreamReader sr = new StreamReader(stream);
                    string jsonResponse = sr.ReadToEnd();
                    sr.Close();
                    return (T)JsonConvert.DeserializeObject(jsonResponse, typeof(T));
                }
                return null;
            }
            catch (Exception e)
            {
                Sitecore.Diagnostics.Log.Error($"APIModule: Exception '{e.Message}' - '{e.StackTrace}'", e);
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}