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
    public class NewsController : Controller
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
        public IActionResult Index(string msg = "")
        {
            ViewBag.msg = msg;
            return View();
        }

        public IActionResult addData(string msg = "")
        {
            ViewBag.msg = msg;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> addData(string N_Title, string N_Categories,
            string N_Content)
        {
            List<string> keyList = getCredentiaInfo();
            var dynamoDBclientObject = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], RegionEndpoint.USEast1);
            var PartitionKey = HttpContext.Session.GetString("_email");
            Dictionary<string, AttributeValue> attributesList = new Dictionary<string, AttributeValue>();
            string message = "";
            try
            {
                attributesList["ProducerName"] = new AttributeValue { S = PartitionKey };
                attributesList["NewsID"] = new AttributeValue { S = Guid.NewGuid().ToString() };
                attributesList["N_Title"] = new AttributeValue { S = N_Title };
                attributesList["N_Categories"] = new AttributeValue { S = N_Categories };
                attributesList["N_Content"] = new AttributeValue { S = N_Content };

                var request = new PutItemRequest
                {
                    TableName = tableName,
                    Item = attributesList
                };

                await dynamoDBclientObject.PutItemAsync(request);

                message = "News has successfully added into DynamoDB";
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error in creating item in DynamoDB: " + ex.Message;
            }
            catch (Exception ex)
            {
                message = "Error in creating item in DynamoDB: " + ex.Message;
            }
            return RedirectToAction("addData", "News", new { msg = message });
        }

        public IActionResult manageNews(string msg = "")
        {
            ViewBag.msg = msg;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> manageNews(string operators,string names)
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
                scanprice.AddCondition("ProducerName", ScanOperator.Equal, name);


                Table customerTransactions = Table.LoadTable(dynamoDBclientObject, tableName);
                Search search = customerTransactions.Scan(scanprice); //keep search sentence

                do
                {
                    receiveReturnValue = await search.GetNextSetAsync();

                    if (receiveReturnValue.Count == 0)
                    {
                        return RedirectToAction("manageNews", "News", new { msg = "Data is not found" });
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
                return RedirectToAction("manageNews", "News", new { msg = "Error in retrieving data: " + ex.Message });
            }
            catch (Exception ex)
            {
                return RedirectToAction("manageNews", "News", new { msg = "Error in retrieving data: " + ex.Message });
            }
        }

        //create a edit form for user to edit the item in the dynamo table
        [HttpGet]
        public async Task<IActionResult> editdata(string prodID, string NewsID)
        {
            List<string> keyList = getCredentiaInfo();
            var dynamoDBclientObject = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], RegionEndpoint.USEast1);

            //list to store result
            List<Document> returnRecords = new List<Document>();

            //get single result and convert it to string
            List<KeyValuePair<string, string>> singleConvertedRecord = new List<KeyValuePair<string, string>>();

            try
            {
                QueryFilter searchSingle = new QueryFilter("ProducerName", QueryOperator.Equal, prodID);
                searchSingle.AddCondition("NewsID", QueryOperator.Equal, NewsID);

                Table custQueryTable = Table.LoadTable(dynamoDBclientObject, tableName);
                Search searchOperation = custQueryTable.Query(searchSingle);

                returnRecords = await searchOperation.GetNextSetAsync();
                if (returnRecords.Count <= 0)
                {
                    return RedirectToAction("manageNews", "News", new { msg = "Error in retrieving " + prodID + "'s data: " });

                }

                foreach (var singledocumentrecord in returnRecords)
                {
                    //convert document type record become to the string, string type - for frontend display usage
                    foreach (var attributeKey in singledocumentrecord.GetAttributeNames())
                    {
                        string attributeValue = "";
                        var value = singledocumentrecord[attributeKey];

                        if (value is DynamoDBBool) //change the type from document type to real string type
                            attributeValue = value.AsBoolean().ToString();
                        else if (value is Primitive)
                            attributeValue = value.AsPrimitive().Value.ToString();
                        else if (value is PrimitiveList)
                            attributeValue = string.Join(",", (from primitive
                                            in value.AsPrimitiveList().Entries
                                                               select primitive.Value).ToArray());

                        singleConvertedRecord.Add(new KeyValuePair<string, string>(attributeKey, attributeValue));
                    }
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                return RedirectToAction("manageNews", "News", new { msg = "Error in retrieving " + prodID + "'s data: " + ex.Message });
            }
            catch (Exception ex)
            {
                return RedirectToAction("manageNews", "News", new { msg = "Error in retrieving " + prodID + "'s data: " + ex.Message });
            }

            return View(singleConvertedRecord);
        }

        //update the data 
        [HttpPost("/News/editdata")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> processData(string ProducerName, string NewsID, string N_Categories, string N_Content, string N_Title)
        {
            List<string> keyList = getCredentiaInfo();
            var dynamoDBclientObject = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], RegionEndpoint.USEast1);

            List<KeyValuePair<string, AttributeValue>> updatelistinfo = new List<KeyValuePair<string, AttributeValue>>();

            updatelistinfo.Add(new KeyValuePair<string, AttributeValue>("N_Categories", new AttributeValue { S = N_Categories }));
            updatelistinfo.Add(new KeyValuePair<string, AttributeValue>("N_Content", new AttributeValue { S = N_Content }));

            string message = "";

            try
            {
                foreach (var attributes in updatelistinfo)
                {
                    var tagvalue = new Dictionary<string, AttributeValue>();
                    tagvalue.Add(":value", attributes.Value);

                    var updateitemrequest = new UpdateItemRequest
                    {
                        TableName = tableName,
                        Key =
                        new Dictionary<string, AttributeValue>()
                        {
                            {"ProducerName", new AttributeValue{ S= ProducerName } },
                            {"NewsID", new AttributeValue{ S= NewsID } },

                        },
                        ExpressionAttributeValues = tagvalue,
                        UpdateExpression = "SET " + attributes.Key + " = :value"

                    };
                    await dynamoDBclientObject.UpdateItemAsync(updateitemrequest);
                }
                message ="News with a title " + N_Title + " is now updated";
            }
            catch (AmazonDynamoDBException ex)
            {
                message = ex.Message;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return RedirectToAction("manageNews", "News", new { msg = message });
        }

        public async Task<IActionResult> deletedata(string prodID, string NewsID)
        {
            string message = "";
            //1. get credential information and link to this function
            List<string> keyList = getCredentiaInfo();
            var dynamoDBclientObject = new AmazonDynamoDBClient(keyList[0], keyList[1], keyList[2], RegionEndpoint.USEast1);

            try
            {
                var deleteitem = new DeleteItemRequest
                {
                    TableName = tableName,
                    Key =
                    new Dictionary<string, AttributeValue>() {
                        {"ProducerName", new AttributeValue { S = prodID }},
                        {"NewsID", new AttributeValue { S = NewsID }}
                    },
                };

                await dynamoDBclientObject.DeleteItemAsync(deleteitem);
                message = "Selected news has been removed";
            }
            catch (AmazonDynamoDBException ex)
            {
                message = "Error: " + ex.Message;
            }
            catch (Exception ex)
            {
                message = "Error: " + ex.Message;
            }
            return RedirectToAction("manageNews", "News", new { msg = message });
        }
    }
}
