using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using EPROCUREMENT.SapResponse;
using EPROCUREMENT.DTO;

namespace EPROCUREMENT.sap
{
	public static class PackagesApi
	{
        public static async Task<List<PackagesResponse>> PostToSapApi(SapPackagesApiDTO sapPackagesApiDTO, string apiUrl)
        {
            // Create JSON body
            var jsonBody = new
            {
                date = new[]
                {
                    new { SIGN = "I", OPTION = "BT", LOW = sapPackagesApiDTO.datefrom, HIGH = sapPackagesApiDTO.dateTo }
                },
                M_GROUP = sapPackagesApiDTO.inudstries.Select(dto => new { MATKL = dto.MtrSrvGrpCode }).ToList()
            };

            List<PackagesResponse> packagesResponses = new List<PackagesResponse>();

            // Serialize JSON body
            var requestBody = JsonConvert.SerializeObject(jsonBody);

            // Create HttpClientHandler with CookieContainer
            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SQE_200", "SAP_SESSIONID_SQE_200"));

            // Create HttpClient with handler
            using (var httpClient = new HttpClient(handler))
            {
                // Set authorization header
                var byteArray = Encoding.ASCII.GetBytes("E-PROC:123456");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                // Create HttpRequestMessage with HttpMethod.Post
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                // Set request content to the serialized JSON body
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Send the request
                var response = await httpClient.SendAsync(request);

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    packagesResponses = JsonConvert.DeserializeObject<List<PackagesResponse>>(responseContent);


                    packagesResponses = packagesResponses
                                          .OrderBy(po => po.PR)                // Primary sorting by PR
                                          .ThenBy(po => po.PR_ITEM)            // Secondary sorting by PRItem
                                          .ToList();
                }

                return packagesResponses;
            }
        }

        public static async Task<List<PackagesResponseService>> PostToSapApiService(SapPackagesApiDTO sapPackagesApiDTO, string apiUrl)
            {
                // Create JSON body
                var jsonBody = new
                {
                    date = new[]
                    {
                    new { SIGN = "I", OPTION = "BT", LOW = sapPackagesApiDTO.datefrom, HIGH = sapPackagesApiDTO.dateTo }
                },
                    //M_GROUP = new[]
                    //{
                    //	new { MATKL = "41406003" },
                    //	//new { MATKL = "10202005" }
                    //}
                    M_GROUP = sapPackagesApiDTO.inudstries.Select(dto => new { MATKL = dto.MtrSrvGrpCode }).ToList()
                };

                List<PackagesResponseService> packagesResponses = new List<PackagesResponseService>();

                // Serialize JSON body
                var requestBody = JsonConvert.SerializeObject(jsonBody);

                //string apiUrl = "http://siac-s4h-qas.siac.local:8000/ze-proq_package?project=10010&ind=1";

                //string apiUrl = "http://62.240.120.107:8000/ze-proq_package?project=10010&ind=1";

                //string apiUrl = "http://10.1.1.57:8050/ze-proq_package?project=50013&ind=1";

                // Create HttpClientHandler with CookieContainer
                var handler = new HttpClientHandler();
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(apiUrl), new Cookie("SAP_SESSIONID_SQE_200", "SAP_SESSIONID_SQE_200"));

                // Create HttpClient with handler
                using (var httpClient = new HttpClient(handler))
                {
                    // Set authorization header
                    var byteArray = Encoding.ASCII.GetBytes("E-PROC:123456");
                   // var byteArray = Encoding.ASCII.GetBytes("E-PROC:In@123");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));


                    // Create HttpRequestMessage with HttpMethod.Post
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    // Set request content to the serialized JSON body
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    // Send the request
                    var response = await httpClient.SendAsync(request);

                    // Handle the response
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        packagesResponses = JsonConvert.DeserializeObject<List<PackagesResponseService>>(responseContent);


                        packagesResponses = packagesResponses
                                           .OrderBy(po => po.PR)                // Primary sorting by PR
                                           .ThenBy(po => po.PR_ITEM)            // Secondary sorting by PRItem
                                           .ToList();
                    }

                    // Return response
                    return packagesResponses;
                }
            }
	}
}