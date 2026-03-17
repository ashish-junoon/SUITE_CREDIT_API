using JC.Experian.ExperianModel;
using System.Security;

namespace JC.Experian
{
    public static class SoapTemplateBuilder
    {
        public static string Build(
        ExperianApiRequest payload,
        string firstName,
        string surname,
        bool EXPERIAN_SERVICES_PROD)
        {
            string username = EXPERIAN_SERVICES_PROD ? ExperianConfig.ProdVariables.SOAP_USERNAME : ExperianConfig.UATVariables.SOAP_USERNAME;
            string password = EXPERIAN_SERVICES_PROD ? ExperianConfig.ProdVariables.SOAP_PASSWORD : ExperianConfig.UATVariables.SOAP_PASSWORD;

            // ⚠️ Always XML-escape user inputs
            firstName = SecurityElement.Escape(firstName);
            surname = SecurityElement.Escape(surname);
            string pan = SecurityElement.Escape(payload.Pan ?? "");
            string mobile = SecurityElement.Escape(payload.Mobile ?? "");

            const string gender = "1"; // Hardcoded
            const string dob = "19850710"; // Hardcoded
            const string FlatNoPlotNoHouseNo = "DEFAULT";
            const string City = "Mumbai";
            const string State = "27";
            const string PinCode = "400001";

            var xmlBody = $@"
<INProfileRequest>
  <Identification>
    <XMLUser>{username}</XMLUser>
    <XMLPassword>{password}</XMLPassword>
  </Identification>
  <Application>
    <FTReferenceNumber>REF123</FTReferenceNumber>
    <CustomerReferenceID>CR123</CustomerReferenceID>
    <EnquiryReason>99</EnquiryReason>
    <FinancePurpose>99</FinancePurpose>
    <AmountFinanced>0</AmountFinanced>
    <DurationOfAgreement>0</DurationOfAgreement>
    <ScoreFlag>1</ScoreFlag>
    <PSVFlag>0</PSVFlag>
  </Application>
  <Applicant>
    <Surname>{surname}</Surname>
    <FirstName>{firstName}</FirstName>
    <GenderCode>{gender}</GenderCode>
    <IncomeTaxPAN>{pan}</IncomeTaxPAN>
    <DateOfBirth>{dob}</DateOfBirth>
    <PhoneNumber>{mobile}</PhoneNumber>
  </Applicant>
<Address>
      <FlatNoPlotNoHouseNo>{FlatNoPlotNoHouseNo}</FlatNoPlotNoHouseNo>
      <City>{City}</City>
      <State>{State}</State>
      <PinCode>{PinCode}</PinCode>
    </Address>
</INProfileRequest>";

            var soapEnvelope = $@"
<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                  xmlns:urn=""http://nextgenws.ngwsconnect.experian.com"">
  <soapenv:Header/>
  <soapenv:Body>
    <urn:process>
      <urn:cbv2String><![CDATA[{xmlBody}]]></urn:cbv2String>
    </urn:process>
  </soapenv:Body>
</soapenv:Envelope>";

            return soapEnvelope.Trim();
        }
    }
}

