using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PradeepTech.DataAccess.Models;
using System.Globalization;

namespace PradeepTech.WebApp.Controllers
{
    public class BaseController : Controller
    {
        public enum NotificationType
        {
            error,

            success,

            warning
        }

        public static string ToTitleCase(string str)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            return myTI.ToTitleCase(str);

            // return str?.First().ToString().ToUpper() + str?.Substring(1).ToLower();
        }

        public async Task NotifyAsync(string Title,
            string Message,

            NotificationType notificationType)
        {
            var msg = new
            {
                message = Message,
                title = Title,
                icon = notificationType.ToString(),
                type = notificationType.ToString(),
                provider = "toaster"//GetProvider()
            };

            TempData["Message"] = JsonConvert.SerializeObject(msg);
        }

        public void Notify(string Title,
            string Message,
            NotificationType notificationType)
        {
            var msg = new
            {
                message = Message,
                title = Title,
                icon = notificationType.ToString(),
                type = notificationType.ToString(),
                provider = "toaster"//GetProvider()
            };

            TempData["Message"] = JsonConvert.SerializeObject(msg);
        }

        //public void Notify(
        //    string Title =null,
        //    string Message = null,
        //    string Provider = null,
        //    NotificationType notificationType =1)
        //{
        //    var msg = new
        //    {
        //        message = Message,
        //        title = Title,
        //        icon = notificationType.ToString(),
        //        type = notificationType.ToString(),
        //        provider = Provider//GetProvider()
        //    };

        //    TempData["Message"] = JsonConvert.SerializeObject(msg);
        //}

        private string GetProvider()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var value = configuration["NotificationProvider"];

            return value;
        }

        private IEnumerable<DataField> ListData = Enumerable.Empty<DataField>();

        public IEnumerable<DataField> TestList(string listFor)
        {
            IEnumerable<DataField> vList = new List<DataField>()
            {
                new DataField { DataValueField = "01" , DataTextField = listFor + " - 1" , DataGroupField=""},
                new DataField { DataValueField = "02" , DataTextField = listFor + " - 2" , DataGroupField=""},
                new DataField { DataValueField = "03" , DataTextField = listFor + " - 3" , DataGroupField=""},
                new DataField { DataValueField = "04" , DataTextField = listFor + " - 4" , DataGroupField=""},
                new DataField { DataValueField = "05" , DataTextField = listFor + " - 5" , DataGroupField=""},
                new DataField { DataValueField = "06" , DataTextField = listFor + " - 6" , DataGroupField=""},
                new DataField { DataValueField = "07" , DataTextField = listFor + " - 7" , DataGroupField=""},
                new DataField { DataValueField = "08" , DataTextField = listFor + " - 8" , DataGroupField=""},
                new DataField { DataValueField = "09" , DataTextField = listFor + " - 9" , DataGroupField=""},
                new DataField { DataValueField = "10" , DataTextField = listFor + " - 10" , DataGroupField=""},
                new DataField { DataValueField = "11" , DataTextField = listFor + " - 11" , DataGroupField=""},
                new DataField { DataValueField = "12" , DataTextField = listFor + " - 12" , DataGroupField=""},
                new DataField { DataValueField = "13" , DataTextField = listFor + " - 13" , DataGroupField=""},
                new DataField { DataValueField = "14" , DataTextField = listFor + " - 14" , DataGroupField=""},
                new DataField { DataValueField = "15" , DataTextField = listFor + " - 15" , DataGroupField=""},
                new DataField { DataValueField = "16" , DataTextField = listFor + " - 16" , DataGroupField=""},
                new DataField { DataValueField = "17" , DataTextField = listFor + " - 17" , DataGroupField=""},
                new DataField { DataValueField = "18" , DataTextField = listFor + " - 18" , DataGroupField=""},
                new DataField { DataValueField = "19" , DataTextField = listFor + " - 19" , DataGroupField=""},
                new DataField { DataValueField = "20" , DataTextField = listFor + " - 20" , DataGroupField=""}
            };
            return vList;
        }
    }
}