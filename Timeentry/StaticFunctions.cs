using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Timeentry.Models;

namespace Timeentry
{
    public static class StaticFunctions
    {
        public static async Task<string> GetToken()
        {
            try
            {                
                string clientId = Environment.GetEnvironmentVariable("DVclientId");
                string clientSecret = Environment.GetEnvironmentVariable("DVclientSecret");
                string tenantId = Environment.GetEnvironmentVariable("DVtenantId");                
                string[] scope = new string[] { Environment.GetEnvironmentVariable("DVscope") };   
                string authority = Environment.GetEnvironmentVariable("DVLoginUrl") + "/" + tenantId;

                var clientApp = ConfidentialClientApplicationBuilder.Create(clientId)
                                    .WithClientSecret(clientSecret)
                                    .WithAuthority(new Uri(authority))
                                    .Build();

                AuthenticationResult authResult = await clientApp.AcquireTokenForClient(scope).ExecuteAsync();
                return authResult.AccessToken;

               
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static IEnumerable<DateTime> EachDay(string startDate, string endDate)
        {
            DateTime from = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
            DateTime thru = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);

            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static async Task<List<string>> getExistRecords(string token, string startDate, string endDate)
        {
            startDate = DateTime.ParseExact(startDate, "yyyy-MM-dd", null).AddDays(-1).ToString("yyyy-MM-dd");
            endDate = DateTime.ParseExact(endDate, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");

            string dataverseUrl = Environment.GetEnvironmentVariable("DVUrl"); 
            var settings = new ODataClientSettings(new Uri(dataverseUrl));

            settings.BeforeRequest += delegate (HttpRequestMessage message)
            {
                message.Headers.Add("Authorization", "Bearer " + token);
            };

            var client = new ODataClient(settings);
            List<string> existlst = new List<string>();
            string filter = "?$filter=cre80_start gt " + startDate + " and cre80_start lt " + endDate;
            var dateEntries = await client.FindEntriesAsync("cre80_msdyn_timeentries" + filter);
            foreach (var dateEntry in dateEntries)
            {
                existlst.Add(dateEntry["cre80_start"].ToString());
            }
            return existlst;
        }

        public static async Task<HttpResponseMessage> CreateRecord(string token, DateTime datetime)
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.cre80_start = datetime.ToString("yyyy-MM-dd"); // datetime;
            timeEntry.cre80_end = datetime.ToString("yyyy-MM-dd"); // datetime;

            var json = JsonConvert.SerializeObject(timeEntry);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            //var url = "https://org2c9fce96.api.crm4.dynamics.com/api/data/v9.2/cre80_msdyn_timeentries";
            string url = Environment.GetEnvironmentVariable("DVUrl") + "/cre80_msdyn_timeentries";            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync(url, data);
            return response;
        }

    }
}
