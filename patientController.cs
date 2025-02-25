using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

public class PatientController : ApiController
{
    private readonly HttpClient _httpClient;

    public PatientController()
    {
        _httpClient = new HttpClient();
    }

    [HttpGet]
    [Route("api/patients")]
    public async Task<IHttpActionResult> GetPatients()
    {
        string apiUrl = "http://www.optimaljo.com/freshuploader.asmx";
        string soapAction = "http://tempuri.org/getJson_select";
        string sqlStr = "SELECT * FROM Patients_Info";

        // SOAP request body
        string soapEnvelope = $@"
        <?xml version=""1.0"" encoding=""utf-8""?>
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
          <soap:Body>
            <getJson_select xmlns=""http://tempuri.org/"">
              <password>OptimalPass_optimaljo05</password>
              <SQlStr>{sqlStr}</SQlStr>
            </getJson_select>
          </soap:Body>
        </soap:Envelope>";

        // Set headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("SOAPAction", soapAction);
        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        try
        {
            // Send SOAP request
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return InternalServerError(new Exception($"HTTP error! Status: {response.StatusCode}"));
            }

            // Read and parse the response
            string responseText = await response.Content.ReadAsStringAsync();
            XDocument xmlDoc = XDocument.Parse(responseText);

            // Extract the result from the XML
            XNamespace ns = "http://tempuri.org/";
            var resultElement = xmlDoc.Descendants(ns + "getJson_selectResult").FirstOrDefault();

            if (resultElement != null)
            {
                string jsonString = resultElement.Value;
                List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(jsonString);

                return Ok(patients);
            }
            else
            {
                return InternalServerError(new Exception("No result found in the response."));
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
}

public class Patient
{
    public int Patient_Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string BirthDate { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string School { get; set; }
    public string MotherName { get; set; }
    public string CreationDate { get; set; }
    public bool IsOnClinic { get; set; }
    public string Status { get; set; }
}
































// using System;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using System.Xml;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json.Linq;

// [Route("api/[controller]")]
// [ApiController]
// public class PatientController : ControllerBase
// {
//     private readonly HttpClient _httpClient;

//     public PatientController(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }

//     [HttpGet("fetchPatients")]
//     public async Task<IActionResult> FetchPatients(string sqlQuery)
//     {
//         string apiUrl = "http://www.optimaljo.com/freshuploader.asmx";  // SOAP API endpoint
//         string soapAction = "http://tempuri.org/getJson_select";         // SOAP action

//         // SOAP XML request body
//         string soapRequest = $@"
//         <?xml version=""1.0"" encoding=""utf-8""?>
//         <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
//                         xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
//                         xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
//           <soap:Body>
//             <getJson_select xmlns=""http://tempuri.org/"">
//               <password>OptimalPass_optimaljo05</password>  
//               <SQlStr>{sqlQuery}</SQlStr>  
//             </getJson_select>
//           </soap:Body>
//         </soap:Envelope>";

//         try
//         {
//             var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
//             {
//                 Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
//             };

//             // Add required SOAP headers
//             request.Headers.Add("SOAPAction", soapAction);

//             HttpResponseMessage response = await _httpClient.SendAsync(request);
//             response.EnsureSuccessStatusCode();
            
//             string responseText = await response.Content.ReadAsStringAsync();

//             // Parse SOAP XML response
//             XmlDocument xmlDoc = new XmlDocument();
//             xmlDoc.LoadXml(responseText);

//             XmlNode resultNode = xmlDoc.GetElementsByTagName("getJson_selectResult")[0];

//             if (resultNode != null)
//             {
//                 string jsonString = resultNode.InnerText;
//                 JArray jsonPatients = JArray.Parse(jsonString);  // Convert to JSON array

//                 return Ok(jsonPatients); // Return JSON to frontend
//             }
//             else
//             {
//                 return BadRequest("No result found in the response.");
//             }
//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, $"Error: {ex.Message}");
//         }
//     }
// }
