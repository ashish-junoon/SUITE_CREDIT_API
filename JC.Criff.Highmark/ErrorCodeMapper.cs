namespace JC.Criff.Highmark
{
    public static class ErrorCodeMapper
    {
        private static readonly Dictionary<string, string> ErrorDescriptions =
            new(StringComparer.OrdinalIgnoreCase)
            {
            { "401", "Authentication failure" },

            { "S00", "Any Transaction Error in inquiry" },
            { "S01", "User Authentication Successful" },
            { "S02", "User Authentication Failure" },
            { "S03", "User Cancelled the Transaction" },
            { "S04", "Authorization for the report ID not complete / Mobile number mismatch" },
            { "S05", "User Validations Failure" },
            { "S06", "Request is accepted by Bureau" },
            { "S07", "Error in request format" },
            { "S08", "Technical Error" },
            { "S09", "No HIT" },
            { "S10", "Auto Authentication – Confident match from Bureau" },
            { "S11", "Authentication Questionnaire phase" }
            };

        public static string GetDescription(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
                return "Invalid error code";

            return ErrorDescriptions.TryGetValue(errorCode, out var desc)
                ? desc
                : "Unknown error code";
        }
    }

}
