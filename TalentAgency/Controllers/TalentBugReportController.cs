using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentAgency.Models;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TalentAgency.Controllers
{
    public class TalentBugReportController : Controller
    {
        private const string QueueName = "TalentAgency";

        private List<string> getKeysInfo()
        {
            List<string> keys = new List<string>();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();

            keys.Add(configuration["AWSCredentialInfo:key1"]);
            keys.Add(configuration["AWSCredentialInfo:key2"]);
            keys.Add(configuration["AWSCredentialInfo:key3"]);

            return keys;
        }

        public IActionResult Index(string msg = "")
        {
            ViewBag.msg = msg;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> sendMessage(string BugDesc)
        {
            List<string> keys = getKeysInfo();
            var sqsclient = new AmazonSQSClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            Bug customer = new Bug
            {
                Bug_Desc = BugDesc,
                Date_created = DateTime.Now
            };
            string message = "";

            try
            {
                SendMessageRequest reserveMessage = new SendMessageRequest
                {
                    MessageBody = JsonConvert.SerializeObject(customer),
                };
                var response = await sqsclient.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = QueueName });
                reserveMessage.QueueUrl = response.QueueUrl;

                await sqsclient.SendMessageAsync(reserveMessage);
                message = "Bug has been reported to the admin";

            }
            catch (AmazonSQSException ex)
            {
                message = ex.Message;
            }
            return RedirectToAction("Index", "TalentBugReport", new { msg = message });

        }
    }
}
