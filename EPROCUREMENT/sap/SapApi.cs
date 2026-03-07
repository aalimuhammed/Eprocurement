using EPROCUREMENT.DTO;
using EPROCUREMENT.Models;
using EPROCUREMENT.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EPROCUREMENT.sap
{
    public class SapApi
    {
        public static async Task<(string, string)> GetcsrfToken()
        {
            var csrfToken = "";
            var sessionIdValue = "";
            string apiUrl = "http://dev-app.siac-construction.com:8000/sap/opu/odata/sap/ZEPROCURMENT_DEEP_INSERTING_V2_SRV/vendorSet";
            // string apiUrl = "http://prd-app.siac-construction.com:8000/sap/opu/odata/sap/ZEPROCURMENT_SIAC_SRV/zvendor_detailsSet('7000000003')?format=json";

            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();

            // handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SDE_310", "-NJQuSguYdfUpnD9FscKpTbZ2S2HnxHusMQAUFadbsU%3d"));
            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("sap-usercontext", "sap-client=310"));

            // Create an HttpClient with the handler
            using (var httpClient = new HttpClient(handler))
            {
                var byteArray = Encoding.ASCII.GetBytes("E-PROC:123456");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // httpClient.DefaultRequestHeaders.Add("x-csrf-token", "lbG8ccdkQAKtoCoAirSxrg==");

                httpClient.DefaultRequestHeaders.Add("x-csrf-token", "Fetch");
                
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                // Send the request
                var response = await httpClient.SendAsync(request);

                // Handle the response as needed
                if (response.IsSuccessStatusCode)
                {
                    // Extract CSRF token from response headers
                    if (response.Headers.TryGetValues("x-csrf-token", out var csrfTokenValues))
                    {
                        csrfToken = csrfTokenValues.FirstOrDefault();
                    }
                    if (response.Headers.TryGetValues("Set-Cookie", out var setCookieValues))
                    {
                        var sapSessionIdCookie = setCookieValues.FirstOrDefault(cookie => cookie.StartsWith("SAP_SESSIONID_SDE_310="));
                        if (sapSessionIdCookie != null)
                        {
                            // Extract the value from the cookie string
                            sessionIdValue = sapSessionIdCookie.Split(';')[0].Substring("SAP_SESSIONID_SDE_310=".Length);
                    
                            // Add the updated cookie to the CookieContainer for subsequent requests
                            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SDE_310", sessionIdValue));
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
                return (csrfToken, sessionIdValue);
            }
        }

        public static async Task<bool> CreateVendorDeepInsertAsync(CreateVendorDeepInsertionDTO vendorData)
        {
            string apiUrl = "http://dev-app.siac-construction.com:8000/sap/opu/odata/sap/ZEPROCURMENT_DEEP_INSERTING_V2_SRV/vendorSet";

            var handler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };

            using var httpClient = new HttpClient(handler);

            // Basic Auth
            var byteArray = Encoding.ASCII.GetBytes("E-PROC:123456");
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Add SAP client cookie
            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("sap-usercontext", "sap-client=110"));

            // Step 1: Fetch CSRF token (must use SAME handler / HttpClient)
            var tokenRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            tokenRequest.Headers.Add("x-csrf-token", "Fetch");

            var tokenResponse = await httpClient.SendAsync(tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
                throw new Exception($"Error fetching CSRF token: {tokenResponse.StatusCode}");

            string csrfToken = tokenResponse.Headers.GetValues("x-csrf-token").FirstOrDefault();

            // Step 2: POST request with CSRF token
            httpClient.DefaultRequestHeaders.Remove("x-csrf-token");
            httpClient.DefaultRequestHeaders.Add("x-csrf-token", csrfToken);

            var industries = vendorData.Industries ?? new List<string>();

            // Build the results array dynamically
            var resultsJson = string.Join(",\n", industries.Select(industry => $@"{{
                    ""SearchTerm1"" : ""{vendorData.Tax_Id?.Trim() ?? ""}"",
                    ""Industry"" : ""{industry.Trim()}""
                }}"));

            string jsonData = $@"{{
    ""d"" : {{
        ""Vendor"" : """",
        ""VendorDes"" : ""{vendorData.Name?.Trim() ?? ""}"",
        ""SearchTerm1"" : ""{vendorData.Tax_Id?.Trim() ?? ""}"",
        ""Telephone"" : ""{vendorData.Telephone?.Trim() ?? ""}"",
        ""Mobile"" : ""{vendorData.Mobile?.Trim() ?? ""}"",
        ""Fax"" : ""{vendorData.Fax?.Trim() ?? ""}"",
        ""Email"" : ""{vendorData.Email?.Trim() ?? ""}"",
        ""Address"" : """",
        ""CommentsSalesPerson"" : ""{vendorData.CommentsSalesPerson?.Trim() ?? ""}"",
        ""ExternalAddressNumberSale"" : ""{vendorData.ExternalAddressNumberSale?.Trim() ?? ""}"",
        ""SalesPersonEmail"" : ""{vendorData.SalesPersonEmail?.Trim() ?? ""}"",
        ""BpType"" : ""{vendorData.BpType?.Trim() ?? ""}"",
        ""CrudType"" : """",
        ""NavVendorToIndustry"" : {{
            ""results"" : [
               {resultsJson}
            ]
        }}
    }}
}}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
                return true;

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine(error);
            return false;
        }
        public static async Task<bool> UpdateVendor(string tokenValue, string sessionValue , string sap_code , VendorData vendorData)
        {
            string vendorId = sap_code;
            // Your API endpoint URL
            string apiUrl = "http://prd-app.siac-construction.com:8000/sap/opu/odata/sap/ZEPROCURMENT_SIAC_SRV/zvendor_detailsSet('{vendorId}')?format=json";
          
         //   string apiUrl = "http://siac-s4h-dev.siac.local:8000/sap/opu/odata/sap/ZEPROCURMENT_SIAC_SRV/zvendor_detailsSet("+7000000003+")?format=json";

            // Create an HttpClientHandler with CookieContainer
            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();

            // Add cookies to the CookieContainer (replace with your actual cookie values)
            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SDE_310", sessionValue));
            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("sap-usercontext", "sap-client=310"));

            using (var httpClient = new HttpClient(handler))
            {
                var byteArray = Encoding.ASCII.GetBytes("E-PROC:In@123");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Include CSRF token in the headers
                httpClient.DefaultRequestHeaders.Add("x-csrf-token", tokenValue);

                string jsonData = "{ \"Vendor\": \""+sap_code+"\", \"Telephone\": \""+vendorData.Telephone+"\", \"Mobile\": \""+vendorData.Mobile+"\", \"Fax\": \""+vendorData.Fax+"\", \"Email\": \""+vendorData.Email+"\", \"Address\": \""+vendorData.Address+"\", \"CommentsSalesPerson\": \""+vendorData.CommentsSalesPerson+"\", \"ExternalAddressNumberSale\": \""+vendorData.ExternalAddressNumberSale+"\", \"SalesPersonEmail\": \""+vendorData.SalesPersonEmail+"\" , \"Industry\": \"S076\", \"BpType\": \"0001\" }";

                // Create an instance of StringContent
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Create an HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl);
                request.Content = content;

                // Send the request
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        //public static async Task<bool> CreateVendorDeepInsertAsync(CreateVendorDeepInsertionDTO vendorData)
        //{
        //    // Step 1: Get CSRF Token + Session
        //    var (tokenValue, sessionValue) = await GetcsrfToken();

        //    if (string.IsNullOrEmpty(tokenValue))
        //        return false;

        //    string apiUrl = "http://dev-app.siac-construction.com:8000/sap/opu/odata/sap/ZEPROCURMENT_DEEP_INSERTING_V2_SRV/vendorSet";

        //    var handler = new HttpClientHandler();
        //    handler.CookieContainer = new CookieContainer();

        //    // Add session cookie
        //    handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SDE_310", sessionValue));
        //    handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("sap-usercontext", "sap-client=310"));

        //    using (var httpClient = new HttpClient(handler))
        //    {
        //        var byteArray = Encoding.ASCII.GetBytes("E-PROC:123456");
        //        httpClient.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        //        // Add CSRF token
        //        httpClient.DefaultRequestHeaders.Add("x-csrf-token", tokenValue);
        //        httpClient.DefaultRequestHeaders.Accept.Add(
        //            new MediaTypeWithQualityHeaderValue("application/json"));

        //        // Deep Insert JSON body
        //        string jsonData = $@"
        //{{
        //  ""d"" : {{
        //    ""Vendor"" : """",
        //    ""VendorDes"" : ""{vendorData.Name}"",
        //    ""SearchTerm1"" : ""{vendorData.Tax_Id}"",
        //    ""Telephone"" : ""{vendorData.Telephone}"",
        //    ""Mobile"" : ""{vendorData.Mobile}"",
        //    ""Fax"" : ""{vendorData.Fax}"",
        //    ""Email"" : ""{vendorData.Email}"",
        //    ""Address"" : """",
        //    ""CommentsSalesPerson"" : ""{vendorData.CommentsSalesPerson}"",
        //    ""ExternalAddressNumberSale"" : ""{vendorData.ExternalAddressNumberSale}"",
        //    ""SalesPersonEmail"" : ""{vendorData.SalesPersonEmail}"",
        //    ""BpType"" : ""{vendorData.BpType}"",
        //    ""CrudType"" : """",
        //    ""NavVendorToIndustry"" : {{
        //      ""results"" : [
        //        {{
        //          ""SearchTerm1"" : ""{vendorData.Tax_Id}"",
        //          ""Industry"" : ""{vendorData.Industries}"",
        //          ""vendor"" : {{
        //            ""__deferred"" : {{
        //              ""uri"" : ""{apiUrl}/industrySet(SearchTerm1='{vendorData.Tax_Id}',Industry='{vendorData.Industries}')/vendor""
        //            }}
        //          }}
        //        }}
        //      ]
        //    }}
        //  }}
        //}}";

        //        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        //        var response = await httpClient.PostAsync(apiUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(error);
        //            return false;
        //        }
        //    }
        //}
        public static async Task SaveToSAPAsync(SapStatusDTO sapStatusDTO)
        {
            //live
            var url = "http://prd-app.siac-construction.com:8000/zpr_status?sap-client=310";

            //sand
           // var url = "http://snd.siac-construction.com:8080/zpr_status?sap-client=310";

			var username = "E-PROC";
          //  var password = "In@123";
            var password = "123456";

            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.CookieContainer.Add(new Uri(url), new Cookie("SAP_SESSIONID_SQE_200", "SAP_SESSIONID_SQE_200"));

            // Create the HttpClient
            using (var client = new HttpClient(handler))
            {
                // Set the basic authentication header
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				// Create the JSON body
				var jsonBody = $@"
                    [
                        {{
                            ""MANDT"": ""310"",
                            ""PR"": ""{sapStatusDTO.PR}"",
                            ""ITEM"": {sapStatusDTO.ITEM},
                            ""SER_ITEM"": {sapStatusDTO.SER_ITEM},
                            ""STATUS"": ""{sapStatusDTO.STATUS}"",
                            ""EPACKAGE"": ""{sapStatusDTO.EPACKAGE}"",
                            ""PO"": ""{sapStatusDTO.PO}"",
                           ""INDUSTRY_DESC"": ""{sapStatusDTO.industry}"",
                            ""PACK_DATE"": ""{DateTime.Now.ToString("yyyyMMdd")}""
                        }}
                    ]";

				// var jsonBody = JsonSerializer.Serialize(sapStatusDTO);

				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the POST request
                try
                {
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    // Read and display the response content
                    var responseContent = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error:");
                    Console.WriteLine(e.Message);
                }
            }

        }
		public static async Task UpdatBiddingToSAPAsync(SapBiddingViewModel biddingViewModel)
		{
			//live
			var url = "http://prd-app.siac-construction.com:8000/zpr_status?sap-client=310";

			//sand
			//var url = "http://snd.siac-construction.com:8080/zpr_status?sap-client=310";

			var username = "E-PROC";
			//  var password = "In@123";
			var password = "123456";

			var handler = new HttpClientHandler();
			handler.CookieContainer = new CookieContainer();
			handler.CookieContainer.Add(new Uri(url), new Cookie("SAP_SESSIONID_SQE_200", "SAP_SESSIONID_SQE_200"));

			// Create the HttpClient
			using (var client = new HttpClient(handler))
			{
				// Set the basic authentication header
				var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				// Create the JSON body
				var jsonBody = $@"
                    [
                        {{
                            ""MANDT"": ""310"",
                            ""PR"": ""{biddingViewModel.PR}"",
                            ""ITEM"": {biddingViewModel.ITEM},
                            ""SER_ITEM"": {biddingViewModel.SER_ITEM},
                            ""ST_QOT_DATE"": ""{DateTime.Now.ToString("yyyyMMdd")}"",
                            ""NO_OF_QOTS"": ""{biddingViewModel.NO_OF_QOTS}""
                        }}
                    ]";

				// var jsonBody = JsonSerializer.Serialize(sapStatusDTO);

				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				// Send the POST request
				try
				{
					var response = await client.PostAsync(url, content);
					response.EnsureSuccessStatusCode();

					// Read and display the response content
					var responseContent = await response.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException e)
				{
					Console.WriteLine("Request error:");
					Console.WriteLine(e.Message);
				}
			}

		}
		public static async Task CancelRevokeSAP(SapCancelRevokeDTO sapCancelRevokeDTO)
        {
            //sand
           // var url = "http://snd.siac-construction.com:8080/zpr_status?sap-client=310";

            //LIVE
            var url = "http://prd-app.siac-construction.com:8000/zepro_can_revok?sap-client=310";

			var username = "E-PROC";
			//var password = "In@123";
            var password = "123456";

            var handler = new HttpClientHandler();
			handler.CookieContainer = new CookieContainer();
			handler.CookieContainer.Add(new Uri(url), new Cookie("SAP_SESSIONID_SQE_200", "SAP_SESSIONID_SQE_200"));

			// Create the HttpClient
			using (var client = new HttpClient(handler))
			{
				// Set the basic authentication header
				var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

				// Create the JSON body
				var jsonBody = $@"
            [
                {{
                    ""PR"": ""{sapCancelRevokeDTO.pr_num}"",
                    ""ITEM"": {sapCancelRevokeDTO.line_item},
                    ""SER_ITEM"": {sapCancelRevokeDTO.ser_item}
                }}
            ]";

				// var jsonBody = JsonSerializer.Serialize(sapStatusDTO);

				//var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				// Send the POST request
				try
				{
					var request = new HttpRequestMessage
					{
						Method = HttpMethod.Delete,
						RequestUri = new Uri(url),
						Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
				    };
					var response = await client.SendAsync(request);
					//var response = await client.PostAsync(url, content);
					response.EnsureSuccessStatusCode();

					// Read and display the response content
					var responseContent = await response.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException e)
				{
					Console.WriteLine("Request error:");
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}