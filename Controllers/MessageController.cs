using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;

using Azure.Core;
using System.Net.WebSockets;
using System.Text;
using System;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Cors;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace messageswebapi.Controllers
{


    /// <summary>
    /// Controller for message read
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly string CosmosDbName = "Websocketdatastore";
        private readonly string CosmosDbContainerName = "Messages";
        private readonly IConfiguration _configuration;

        /// <summary>
        /// configuration for message controller
        /// </summary>
        /// <param name="configuration"></param>
        public  MessageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        /// <summary>
        /// connector client with configuraion
        /// </summary>
        /// <returns></returns>
        private Container ContainerClient()
        {

            string CosmosDBAccountUri = _configuration.GetValue<string>("CosmosDBAccountUri");
            string CosmosDBAccountPrimaryKey = _configuration.GetValue<string> ("CosmosDBAccountPrimaryKey");

            CosmosClient cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
            Container containerClient = cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return containerClient;
        }

        
       
        /// <summary>
        /// Get all messages for cosmos database
        /// 
        /// </summary>
        /// <returns>json string from the Cosmos database</returns>
        [HttpGet(Name = "GetAllMessages")]
        public async Task<IActionResult> GetAllMessages()
        {
            try
            {
                var container = ContainerClient();
                var sqlQuery = "SELECT * FROM messages";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Socketmesssages> queryResultSetIterator = container.GetItemQueryIterator<Socketmesssages>(queryDefinition);
                List<Socketmesssages> messages = new List<Socketmesssages>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Socketmesssages> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Socketmesssages message in currentResultSet)
                    {
                        messages.Add(message);
                    }
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Method insert data into cosmos db
        /// </summary>
        /// <param name="message">JSON string from web socket client</param>
        /// <returns>return response with status 200</returns>
        [HttpPost]
        public async Task<IActionResult> AddMessages(Socketmesssages message)
        {
            try
            {
                var container = ContainerClient();
                var response = await container.CreateItemAsync(message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Get all records with matching machine id
        /// </summary>
        /// <param name="machineId">machine id string</param>
        /// <returns>JSON string with data from cosmos DB</returns>

        [HttpGet]
        public async Task<IActionResult> GetbyMachineID(string machineId)
        {
            try
            {
                var container = ContainerClient();
             
                var sqlQuery = string.Format("SELECT * FROM messages where  messages.machine_id='{0}'", machineId);
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Socketmesssages> queryResultSetIterator = container.GetItemQueryIterator<Socketmesssages>(queryDefinition);
                List<Socketmesssages> messages = new List<Socketmesssages>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Socketmesssages> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Socketmesssages message in currentResultSet)
                    {
                        messages.Add(message);
                    }
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///   Get all records with matching message id
        /// </summary>
        /// <param name="messagesId">message id for filter</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetMessagesById(string messagesId)
        {
            try
            {
                
                var container = ContainerClient();
                var sqlQuery = string.Format ("SELECT * FROM messages where  messages.id='{0}'" , messagesId) ;
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Socketmesssages> queryResultSetIterator = container.GetItemQueryIterator<Socketmesssages>(queryDefinition);
                List<Socketmesssages> messages = new List<Socketmesssages>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Socketmesssages> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Socketmesssages message in currentResultSet)
                    {
                        messages.Add(message);
                    }
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Get All message by status field.
        /// </summary>
        /// <param name="status">status with</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> GetMessagesByStatus(messagestatus status)
        {
            try
            {
                var container = ContainerClient();
                var sqlQuery = string.Format("SELECT * FROM messages where messages.status='{0}'", status);
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<Socketmesssages> queryResultSetIterator = container.GetItemQueryIterator<Socketmesssages>(queryDefinition);
                List<Socketmesssages> messages = new List<Socketmesssages>();
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Socketmesssages> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Socketmesssages message in currentResultSet)
                    {
                        messages.Add(message);
                    }
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// update cosmos db from existing message (Not implemented from client) 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateMessages(Socketmesssages message)
        {
            try
            {
                var container = ContainerClient();
                ItemResponse<Socketmesssages> res = await container.ReadItemAsync<Socketmesssages>(message.id , new Microsoft.Azure.Cosmos.PartitionKey("id"));
                //Get Existing Item
                var existingItem = res.Resource;
                //Replace existing item values with new values
                existingItem.machine_id = message.machine_id;
                existingItem.status = message.status;
              
                var updateRes = await container.ReplaceItemAsync(existingItem, message.id, new Microsoft.Azure.Cosmos.PartitionKey("id"));
                return Ok(updateRes.Resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
