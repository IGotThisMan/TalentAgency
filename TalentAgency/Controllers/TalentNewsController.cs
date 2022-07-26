using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TalentAgency.Controllers
{
    public class TalentNewsController : Controller
    {

        private const string tableName = "TalentAgency";

        private List<string> getCredentiaInfo()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();

            List<string> keyList = new List<string>();
            keyList.Add(configure["AWSCredentialInfo:key1"]);
            keyList.Add(configure["AWSCredentialInfo:key2"]);
            keyList.Add(configure["AWSCredentialInfo:key3"]);

            return keyList;
        }

        

        public async Task<IActionResult> Index(string operators, string category)
        {
            List<string> keyList = getCredentiaInfo();
            var dynamoDBclientObject = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], RegionEndpoint.USEast1);

            List<Document> receiveReturnValue = new List<Document>();

            List<KeyValuePair<string, string>> singleDocument = new List<KeyValuePair<string, string>>();

            List<List<KeyValuePair<string, string>>> fullList = new List<List<KeyValuePair<string, string>>>();

            var name = HttpContext.Session.GetString("_email");
            try
            {
                ScanFilter scanprice = new ScanFilter();
                if (operators == "=")
                    scanprice.AddCondition("N_Categories", ScanOperator.Equal, category);


                Table customerTransactions = Table.LoadTable(dynamoDBclientObject, tableName);
                Search search = customerTransactions.Scan(scanprice); //keep search sentence

                do
                {
                    receiveReturnValue = await search.GetNextSetAsync();

                    if (receiveReturnValue.Count == 0)
                    {
                        return RedirectToAction("Index", "TalentNews", new { msg = "Data is not found" });
                    }

                    foreach (var singleitem in receiveReturnValue)
                    {
                        foreach (var attributeKey in singleitem.GetAttributeNames())
                        {
                            string attributeValue = "";

                            if (singleitem[attributeKey] is DynamoDBBool) //change the type from document type to real string type
                                attributeValue = singleitem[attributeKey].AsBoolean().ToString();
                            else if (singleitem[attributeKey] is Primitive)
                                attributeValue = singleitem[attributeKey].AsPrimitive().Value.ToString();
                            else if (singleitem[attributeKey] is PrimitiveList)
                                attributeValue = string.Join(",", (from primitive
                                                in singleitem[attributeKey].AsPrimitiveList().Entries
                                                                   select primitive.Value).ToArray());

                            singleDocument.Add(new KeyValuePair<string, string>(attributeKey, attributeValue));
                        }

                        fullList.Add(singleDocument);
                        singleDocument = new List<KeyValuePair<string, string>>();
                    }
                }
                while (!search.IsDone);
                ViewBag.msg = "Data is found";
                return View(fullList);
            }
            catch (AmazonDynamoDBException ex)
            {
                return RedirectToAction("Index", "TalentNews", new { msg = "Error in retrieving data: " + ex.Message });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "TalentNews", new { msg = "Error in retrieving data: " + ex.Message });
            }
        }
    }
}
