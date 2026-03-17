using CIC.Helper;
using CIC.Model.Criff.Request;
using CIC.Model.Criff.Response;
using iText.Layout.Borders;
using LoggerLibrary;
using Org.BouncyCastle.Ocsp;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace JC.Criff.Highmark
{
    public class CirffServiceApp : ICirffServiceApp
    {
        private readonly ILoggerManager _logger;
        private readonly HttpClient _http;
        private static string CRIF_MBRID = "";
        private static string CRIF_APPID = "";
        private static string CRIF_USER_ID = "";
        private static string CRIF_PRODUCT_CODE = "";
        private static string CRIF_PASSWORD = "";
        private static string CRIF_CUSTOMER_NAME = "";
        private static string CRIF_ENDPOINT_URL = "";
        ResponseStages stageOne = new();
        public CirffServiceApp(IHttpClientFactory factory, ILoggerManager logger)
        {
            _http = factory.CreateClient();
            _http.Timeout = TimeSpan.FromSeconds(30);
            _logger = logger;
        }

        public async Task<FusionParsedResponse> CriffPrefil(CrifPrefillRQ requestBody, bool CRIF_FUSION_PROD, string company_id)
        {
            CRIF_MBRID = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_MBRID : CrifFusionConfig.UATVariables.CRIF_MBRID;
            CRIF_APPID = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_APPID : CrifFusionConfig.UATVariables.CRIF_APPID;
            CRIF_USER_ID = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_USER_ID : CrifFusionConfig.UATVariables.CRIF_USER_ID;
            CRIF_PRODUCT_CODE = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_PRODUCT_CODE : CrifFusionConfig.UATVariables.CRIF_PRODUCT_CODE;
            CRIF_PASSWORD = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_PASSWORD : CrifFusionConfig.UATVariables.CRIF_PASSWORD;
            CRIF_CUSTOMER_NAME = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_CUSTOMER_NAME : CrifFusionConfig.UATVariables.CRIF_CUSTOMER_NAME;
            CRIF_ENDPOINT_URL = CRIF_FUSION_PROD ? CrifFusionConfig.ProdVariables.CRIF_FUSION_ENDPOINT_URL : CrifFusionConfig.UATVariables.CRIF_FUSION_ENDPOINT_URL;

            try
            {
                var uniq = CIC.Helper.CommonClass.GenerateUniqueRequestNo();
                string requestXML = $@"<REQUEST-REQUEST-FILE>
                        <HEADER-SEGMENT>
                                <PRODUCT-TYP>FUSION</PRODUCT-TYP>
                                <PRODUCT-VER>2.0</PRODUCT-VER>
                                <REQ-MBR>{CRIF_MBRID}</REQ-MBR>
                                <SUB-MBR-ID>{CRIF_CUSTOMER_NAME}</SUB-MBR-ID>
                                <INQ-DT-TM>{uniq.datetime}</INQ-DT-TM>
                                <REQ-VOL-TYP>C01</REQ-VOL-TYP>
                                <REQ-ACTN-TYP>AT01</REQ-ACTN-TYP>
                                <TEST-FLG>HMTEST</TEST-FLG>
                                <AUTH-FLG>Y</AUTH-FLG>
                                <RES-FRMT>XML/HTML</RES-FRMT>
                                <RES-FRMT-EMBD>Y</RES-FRMT-EMBD>
                                <MEMBER-PRE-OVERRIDE>Y</MEMBER-PRE-OVERRIDE>
                                <LOS-NAME>BALIC</LOS-NAME>
                                <REQ-SERVICE-TYPE>DEMOG|CB SCORE</REQ-SERVICE-TYPE>
                        </HEADER-SEGMENT>
                        <INQUIRY>
                                <APPLICANT-SEGMENT>
                                    <NAME>{requestBody.customerName}</NAME>
                                    <DOB-DATE></DOB-DATE>
                                    <PAN></PAN><VOTER-ID></VOTER-ID>
                                    <ADDRESSES><ADDRESS>
                                    <TYPE>D01</TYPE><ADDRESS-1></ADDRESS-1><CITY></CITY><STATE></STATE><PIN></PIN>
                                    </ADDRESS></ADDRESSES>
                                    <PHONE>{requestBody.mobile}</PHONE><EMAILS><EMAIL></EMAIL></EMAILS><IDS><ID><TYPE></TYPE><VALUE></VALUE></ID><ID><TYPE></TYPE><VALUE></VALUE></ID></IDS>
                                    <RELATION-TYPE>K01</RELATION-TYPE><RELATION-VALUE></RELATION-VALUE><RELATION-TYPE>K07</RELATION-TYPE>
                                    <RELATION-VALUE></RELATION-VALUE><NOMINEE-TYPE></NOMINEE-TYPE><NOMINEE-VALUE></NOMINEE-VALUE>
                                    <GENDER-TYPE>G01</GENDER-TYPE></APPLICANT-SEGMENT><APPLICATION-SEGMENT>
                                    <INQUIRY-UNIQUE-REF-NO>{uniq.random}</INQUIRY-UNIQUE-REF-NO>
                                    <CREDT-RPT-ID></CREDT-RPT-ID>
                                    <CREDT-REQ-TYP>INDV</CREDT-REQ-TYP>
                                    <CREDT-INQ-PURPS-TYP>CP01</CREDT-INQ-PURPS-TYP>
                                    <CREDT-INQ-PURPS-TYP-DESC></CREDT-INQ-PURPS-TYP-DESC>
                                    <CLIENT-CUSTOMER-ID></CLIENT-CUSTOMER-ID><BRANCH-ID></BRANCH-ID><APP-ID></APP-ID><AMOUNT></AMOUNT></APPLICATION-SEGMENT>
                        </INQUIRY></REQUEST-REQUEST-FILE>";




                var request = new HttpRequestMessage(HttpMethod.Post, CRIF_ENDPOINT_URL);
                request.Headers.Add("requestXml", requestXML.Replace("\r", "").Replace("\n", ""));
                request.Headers.Add("UserId", CRIF_USER_ID);
                request.Headers.Add("password", CRIF_PASSWORD);
                request.Headers.Add("mbrid", CRIF_MBRID);
                request.Headers.Add("productType", "FUSION");
                request.Headers.Add("productVersion", "2.0");
                request.Headers.Add("reqVolType", "C01");
                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return await Task.FromResult(new FusionParsedResponse { success = false, StatusCode = (int)response.StatusCode, Message= $"EXCEPTION" });
                }
                var xmlResponse = await response.Content.ReadAsStringAsync();

                ReportFile result =  XmlHelper.DeserializeXml<ReportFile>(xmlResponse);

                await Task.Delay(1000);

                string requestXML1 = $@"<REQUEST-REQUEST-FILE>
                                    <HEADER-SEGMENT>
                                    <PRODUCT-TYP>FUSION</PRODUCT-TYP>
                                    <PRODUCT-VER>2.0</PRODUCT-VER>
                                    <REQ-MBR>{CRIF_MBRID}</REQ-MBR>
                                    <SUB-MBR-ID>{CRIF_CUSTOMER_NAME}</SUB-MBR-ID>
                                    <INQ-DT-TM>{uniq.datetime}</INQ-DT-TM>
                                    <REQ-VOL-TYP>C01</REQ-VOL-TYP>
                                    <REQ-ACTN-TYP>AT01</REQ-ACTN-TYP>
                                    <TEST-FLG>HMTEST</TEST-FLG>
                                    <AUTH-FLG>Y</AUTH-FLG>
                                    <RES-FRMT>XML/HTML</RES-FRMT>
                                    <RES-FRMT-EMBD>N</RES-FRMT-EMBD>
                                    <LOS-NAME>MAXLOS</LOS-NAME>
                                    <REQ-SERVICE-TYPE>ALL</REQ-SERVICE-TYPE>
                                    </HEADER-SEGMENT>
                                    <INQUIRY>
                                    <INQUIRY-UNIQUE-REF-NO>{uniq.random}</INQUIRY-UNIQUE-REF-NO>
                                    <REQUEST-DT-TM>{uniq.datetime}</REQUEST-DT-TM>
                                    <REPORT-ID>{result?.InquiryStatus.Inquiry.ReportId}</REPORT-ID>
                                    </INQUIRY>
                                    </REQUEST-REQUEST-FILE>";


                var request1 = new HttpRequestMessage(HttpMethod.Post, CRIF_ENDPOINT_URL);
                request1.Headers.Add("requestXml", requestXML1.Replace("\r", "").Replace("\n", ""));
                request1.Headers.Add("UserId", CRIF_USER_ID);
                request1.Headers.Add("password", CRIF_PASSWORD);
                request1.Headers.Add("mbrid", CRIF_MBRID);
                request1.Headers.Add("productType", "FUSION");
                request1.Headers.Add("productVersion", "2.0");
                request1.Headers.Add("reqVolType", "INDV");

                var response2 = await _http.SendAsync(request1);
                if (!response2.IsSuccessStatusCode) {
                    return await Task.FromResult(new FusionParsedResponse { success = false, StatusCode = (int)response.StatusCode, Message = $"EXCEPTION" });
                }
                var finalResponse = await response2.Content.ReadAsStringAsync();

                var parsed = FusionResponseParser.Parse(finalResponse);
                parsed.success = true;
                parsed.Message = "success";
                return parsed;
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FusionParsedResponse { success = false, StatusCode = 400, Message = $"EXCEPTION : {ex.Message}" });
            }
        }

        public async Task<dynamic> GetCreditReportAsync(SoftPullRQ requestBody, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            string request = string.Empty;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string orderId = Guid.NewGuid().ToString("N")[..8];
            CRIF_MBRID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_MBRID : CrifConfig.UATVariables.CRIF_MBRID;
            CRIF_APPID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_APPID : CrifConfig.UATVariables.CRIF_APPID;
            CRIF_USER_ID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_USER_ID : CrifConfig.UATVariables.CRIF_USER_ID;
            CRIF_PRODUCT_CODE = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PRODUCT_CODE : CrifConfig.UATVariables.CRIF_PRODUCT_CODE;
            CRIF_PASSWORD = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PASSWORD : CrifConfig.UATVariables.CRIF_PASSWORD;
            CRIF_CUSTOMER_NAME = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_CUSTOMER_NAME : CrifConfig.UATVariables.CRIF_CUSTOMER_NAME;
            CRIF_ENDPOINT_URL = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_ENDPOINT_URL : CrifConfig.UATVariables.CRIF_ENDPOINT;
            // string request = $"{requestBody.first_name}||{requestBody.last_name}||{requestBody.dob}|||{requestBody.mobile_number}|||{requestBody.email}||{requestBody.pan_number}|||||||||||{requestBody.address} {requestBody.village} {requestBody.city}, {requestBody.pincode}, {requestBody.state}|{requestBody.city}|{requestBody.city}|{requestBody.state}|{requestBody.pincode}|India|||||||{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|Y|";
            if (requestBody.uid_number.Length == 10)
            {
                request = $"{requestBody.first_name}||{requestBody.last_name}||05-07-1970|||{requestBody.mobile_number}|||test@gmail.com||{requestBody.uid_number}||||||||||||||||India|||||||{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|Y|";
            }
            else
            {
                request = $"{requestBody.first_name}||{requestBody.last_name}||05-07-1970|||{requestBody.mobile_number}|||test@gmail.com|||||||{requestBody.uid_number}|||||||||||India|||||||{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|Y|";
            }
            _logger.LogInfo($"Main Request : {request}");
            string accessCodeRaw =
                $"{CRIF_USER_ID}|{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|{CRIF_PASSWORD}|{CIC.Helper.CommonClass.GetISTTimestamp()}";

            string accessCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(accessCodeRaw));

            #region Stage 1
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            var result = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}initiate", request, keyValuePairs, "text/plain");

            //var jObject = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadLine());
            //Console.WriteLine((string)jObject["albums"][0]["cover_image_url"]);

            stageOne = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseStages>(result);
            _logger.LogInfo($"Response Stage 1 : {result}");
            #endregion
            if (stageOne?.status != "S06")
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }

            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("requestType", "Authorization");
            keyValuePairs.Add("reportId", stageOne.reportId);
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            //-------Stage 2 --------------
            string followUpBody = $"{orderId}|{stageOne.reportId}|{accessCode}|{stageOne.redirectURL}|N|N|Y";
            _logger.LogInfo($"Request Stage 2 : {followUpBody}");
            stageOne = new ResponseStages();
            var result1 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Response Stage 2 : {result1}");
            stageOne = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseStages>(result1);

            if (stageOne.status == "S11" || stageOne.status == "S02") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            else if (stageOne.status == "S08" || stageOne.status == "S07") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }

            else if (stageOne.status != "S09" && stageOne.status != "S10") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            //-------Stage 3 --------------
            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("reportId", stageOne.reportId);
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            _logger.LogInfo($"Request Stage 3 : {followUpBody}");
            var result2 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Response Stage 3 : {result2}");
            return result2;
        }

        public async Task<dynamic> GetCreditReportV1Async(SoftPullRQV1 requestBody, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            string request = string.Empty;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string orderId = Guid.NewGuid().ToString("N")[..8];
            CRIF_MBRID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_MBRID : CrifConfig.UATVariables.CRIF_MBRID;
            CRIF_APPID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_APPID : CrifConfig.UATVariables.CRIF_APPID;
            CRIF_USER_ID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_USER_ID : CrifConfig.UATVariables.CRIF_USER_ID;
            CRIF_PRODUCT_CODE = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PRODUCT_CODE : CrifConfig.UATVariables.CRIF_PRODUCT_CODE;
            CRIF_PASSWORD = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PASSWORD : CrifConfig.UATVariables.CRIF_PASSWORD;
            CRIF_CUSTOMER_NAME = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_CUSTOMER_NAME : CrifConfig.UATVariables.CRIF_CUSTOMER_NAME;
            CRIF_ENDPOINT_URL = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_ENDPOINT_URL : CrifConfig.UATVariables.CRIF_ENDPOINT;
            // string request = $"{requestBody.first_name}||{requestBody.last_name}||{requestBody.dob}|||{requestBody.mobile_number}|||{requestBody.email}||{requestBody.pan_number}|||||||||||{requestBody.address} {requestBody.village} {requestBody.city}, {requestBody.pincode}, {requestBody.state}|{requestBody.city}|{requestBody.city}|{requestBody.state}|{requestBody.pincode}|India|||||||{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|Y|";

            request = $"{requestBody.first_name}||{requestBody.last_name}||05-07-1970|||{requestBody.mobile_number}|||test@gmail.com||{requestBody.pan_number}|||||{requestBody.aadhaar_number}|||||||||||India|||||||{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|Y|";

            _logger.LogInfo($"Main Request : {request}");
            string accessCodeRaw =
                $"{CRIF_USER_ID}|{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|{CRIF_PASSWORD}|{CIC.Helper.CommonClass.GetISTTimestamp()}";

            string accessCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(accessCodeRaw));

            #region Stage 1
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            var result = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}initiate", request, keyValuePairs, "text/plain");

            //var jObject = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadLine());
            //Console.WriteLine((string)jObject["albums"][0]["cover_image_url"]);

            stageOne = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseStages>(result);
            _logger.LogInfo($"Response Stage 1 : {result}");
            #endregion
            if (stageOne?.status != "S06")
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }

            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("requestType", "Authorization");
            keyValuePairs.Add("reportId", stageOne.reportId);
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            //-------Stage 2 --------------
            string followUpBody = $"{orderId}|{stageOne.reportId}|{accessCode}|{stageOne.redirectURL}|N|N|Y";
            _logger.LogInfo($"Request Stage 2 : {followUpBody}");
            stageOne = new ResponseStages();
            var result1 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Response Stage 2 : {result1}");
            stageOne = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseStages>(result1);

            if (stageOne.status == "S11" || stageOne.status == "S02") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            else if (stageOne.status == "S08" || stageOne.status == "S07") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }

            else if (stageOne.status != "S09" && stageOne.status != "S10") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            //-------Stage 3 --------------
            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("reportId", stageOne.reportId);
            keyValuePairs.Add("orderId", orderId);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            _logger.LogInfo($"Request Stage 3 : {followUpBody}");
            var result2 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Response Stage 3 : {result2}");
            return result2;
        }

        public async Task<dynamic> AuthQuestionnaireCriff(AuthRQ requestBody, bool CRIF_HIGHMARK_SERVICES_PROD)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            CRIF_MBRID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_MBRID : CrifConfig.UATVariables.CRIF_MBRID;
            CRIF_APPID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_APPID : CrifConfig.UATVariables.CRIF_APPID;
            CRIF_USER_ID = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_USER_ID : CrifConfig.UATVariables.CRIF_USER_ID;
            CRIF_PRODUCT_CODE = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PRODUCT_CODE : CrifConfig.UATVariables.CRIF_PRODUCT_CODE;
            CRIF_PASSWORD = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_PASSWORD : CrifConfig.UATVariables.CRIF_PASSWORD;
            //CRIF_CUSTOMER_NAME = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_CUSTOMER_NAME : CrifConfig.UATVariables.CRIF_CUSTOMER_NAME;
            CRIF_ENDPOINT_URL = CRIF_HIGHMARK_SERVICES_PROD ? CrifConfig.ProdVariables.CRIF_ENDPOINT_URL : CrifConfig.UATVariables.CRIF_ENDPOINT;


            string accessCodeRaw =
               $"{CRIF_USER_ID}|{CRIF_MBRID}|{CRIF_PRODUCT_CODE}|{CRIF_PASSWORD}|{CIC.Helper.CommonClass.GetISTTimestamp()}";

            string accessCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(accessCodeRaw));

            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("requestType", "Authorization");
            keyValuePairs.Add("reportId", requestBody.reportId);
            keyValuePairs.Add("orderId", requestBody.orderid);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            //-------Stage 2 --------------
            string followUpBody = $"{requestBody.orderid}|{requestBody.reportId}|{accessCode}|https://cir.crifhighmark.com/Inquiry/B2B/secureService.action|N|N|Y|{requestBody.amsware.Trim()}";
            _logger.LogInfo($"Auth Verify Request Stage 2 : {followUpBody}");
            stageOne = new ResponseStages();
            var result1 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Auth Verify Response Stage 2 : {result1}");
            stageOne = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseStages>(result1);

            if (stageOne.status == "S11" || stageOne.status == "S02") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            else if (stageOne.status == "S08" || stageOne.status == "S07") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }

            else if (stageOne.status != "S09" && stageOne.status != "S10" && stageOne.status != "S01") //|| stageOne.status != "S10"
            {
                stageOne.statusDesc = ErrorCodeMapper.GetDescription(stageOne?.status);
                return stageOne;
            }
            //-------Stage 3 --------------
            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("reportId", requestBody.reportId);
            keyValuePairs.Add("orderId", requestBody.orderid);
            keyValuePairs.Add("accessCode", accessCode);
            keyValuePairs.Add("appID", CRIF_APPID);
            keyValuePairs.Add("merchantID", CRIF_MBRID);
            _logger.LogInfo($"Auth Verify Request Stage 3 : {followUpBody}");
            var result2 = await HttpClientPost.PostAsync($"{CRIF_ENDPOINT_URL}response", followUpBody, keyValuePairs, "application/xml");
            _logger.LogInfo($"Auth Verify Response Stage 3 : {result2}");
            return result2;
        }


    }
}
