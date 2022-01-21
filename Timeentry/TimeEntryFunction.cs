using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Timeentry.Models;
using System.IO;
using Newtonsoft.Json;

namespace Timeentry
{
    public static class TimeEntryFunction
    {
        [FunctionName("msdyn_timeentry")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                OperationResponse opeation = new OperationResponse();
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Msdyn_timeentry reqData = JsonConvert.DeserializeObject<Msdyn_timeentry>(requestBody);


                string token = await StaticFunctions.GetToken();
                List<string> existlst = await StaticFunctions.getExistRecords(token, reqData.StartOn, reqData.EndOn);
                opeation.ExistRecords = string.Join(" , ", existlst);

                foreach (DateTime day in StaticFunctions.EachDay(reqData.StartOn, reqData.EndOn))
                {
                    string strday = day.ToString("yyyy-MM-dd");
                    if (!existlst.Contains(strday))
                    {
                        var result = await StaticFunctions.CreateRecord(token, day);
                        if (result.IsSuccessStatusCode)
                        {
                            opeation.CreatedRecords = opeation.CreatedRecords + " , " + strday;
                        }
                        else
                        {
                            opeation.FailedRecords = opeation.CreatedRecords + " , " + strday;
                        }
                    }
                }

                return new OkObjectResult(opeation);
            }
            catch (Exception e)
            {               
                log.LogInformation("Error : " + e.Message);
                return new BadRequestObjectResult(e.Message);       
            }
        }

    }
}
