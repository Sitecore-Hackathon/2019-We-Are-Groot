using Hackathon.Boilerlate.Api.Areas.Model;
using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using Sitecore.SecurityModel;
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
        public ActionResult GetJsonResults()
        {
            var APIData = RenderingContext.Current.Rendering.Item;
            APIModule datasource = new APIModule();
            datasource.APIUrl = APIData.Fields["URL"].ToString();
            datasource.SelectedHeaderValues = APIData.Fields["Select Header Parameters"];
            datasource.HeaderValues = selectedMultilistValues(datasource.SelectedHeaderValues);
            datasource.SelectedInputList = APIData.Fields["Select Input Parameters"];
            datasource.InputParameters = selectedChildMultilistValues(datasource.SelectedInputList);

            var apiResult = InvokeJson<dynamic>(datasource);
            string JsonResult = "Result:" +Convert.ToString(apiResult);
            datasource.OutputParameters = JsonResult;
            Log.Info("Before Editing", this);
            using (new SecurityDisabler())
            {
                APIData.Editing.BeginEdit();
                APIData.Fields["ApiOutput"].Value = JsonResult;
                APIData.Editing.EndEdit();
            }
            Log.Info("After Editing", this);
            return View("~/Areas/Views/APIModule.cshtml", datasource);
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
        public string selectedChildMultilistValues(MultilistField data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            foreach (var items in data.GetItems())
            {
                sb.AppendLine(string.Format(@"""{0}"":""{1}"",", items.Fields["Key"].ToString(), items.Fields["Value"].ToString()));
            }
            sb.AppendLine("}");
            return sb.ToString();
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