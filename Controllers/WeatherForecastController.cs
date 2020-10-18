using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingApi.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        // A Protobuf JSON parser configured to ignore unknown fields. This makes
        // the action robust against new fields being introduced by Dialogflow.
        private static readonly JsonParser jsonParser =
        new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
        [HttpPost]
        public async Task<ContentResult> DialogActionAsync()
        {
            // Parse the body of the request using the Protobuf JSON parser,
            // *not* Json.NET.
            string requestJson;
            using (TextReader reader = new StreamReader(Request.Body))
            {
                requestJson = await reader.ReadToEndAsync();
            }

            WebhookRequest request;
         
                request = jsonParser.Parse<WebhookRequest>(requestJson);
            
            double totalAmount = 0;
            string destination = "";
            double totalPeople = 0;
            
            var destinationPrices = new Dictionary<string, double>() { { "new york", 99.99 }, { "london", 20.50 } , { "riga", 18.95 } , { "paris", 120 } };
            if (request.QueryResult.Action == "book")
{
                //Parse the intent params
           
                var requestParameters = request.QueryResult.Parameters;
                totalPeople = requestParameters.Fields["totalPeople"].NumberValue;
                destination = requestParameters.Fields["destination"].StringValue;
                totalAmount = destinationPrices[destination.ToLower()] * totalPeople;
                
                
                

            }
            // Populate the response
            WebhookResponse response = new WebhookResponse
            {
                FulfillmentText = $"Thank you for the booking. Ticket for {totalPeople} to {destination} will cost {totalAmount} euros."
};
        // Ask Protobuf to format the JSON to return.
        // Again, we don’t want to use Json.NET — it doesn’t know how to handle Struct
        // values etc.
        string responseJson = response.ToString();
return Content(responseJson, "application/json");
    }
}
}
