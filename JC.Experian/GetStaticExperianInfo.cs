namespace CIC.Experian
{
    public static class GetStaticExperianInfo
    {
        public static string GetPaymentFrequency(string frequencyCode)
        {
            switch (frequencyCode)
            {
                case "D": return "Daily";
                case "W": return "Weekly";
                case "F": return "Bi-Weekly";
                case "M": return "Monthly";
                case "Q": return "Quarterly";
                case "H": return "Half-Yearly";
                case "Y": return "Yearly";
                case "OD": return "On-Demand";
                case "BP": return "Bullet Payment";
                default: return "Unknown Payment Frequency";
            }
        }

        public static string GetGenderType(string genderType)
        {
            switch (genderType)
            {
                case "1":
                    return "Male";
                case "2":
                    return "Female";
                case "3":
                    return "Transgender";
                case "4":
                default:
                    return "Unknown";
            }
        }

        public static string GetStateType(string stateCode)
        {
            switch (stateCode)
            {
                case "01": return "JAMMU and KASHMIR";
                case "02": return "HIMACHAL PRADESH";
                case "03": return "PUNJAB";
                case "04": return "CHANDIGARH";
                case "05": return "UTTRANCHAL";
                case "06": return "HARAYANA";
                case "07": return "DELHI";
                case "08": return "RAJASTHAN";
                case "09": return "UTTAR PRADESH";
                case "10": return "BIHAR";
                case "11": return "SIKKIM";
                case "12": return "ARUNACHAL PRADESH";
                case "13": return "NAGALAND";
                case "14": return "MANIPUR";
                case "15": return "MIZORAM";
                case "16": return "TRIPURA";
                case "17": return "MEGHALAYA";
                case "18": return "ASSAM";
                case "19": return "WEST BENGAL";
                case "20": return "JHARKHAND";
                case "21": return "ORRISA";
                case "22": return "CHHATTISGARH";
                case "23": return "MADHYA PRADESH";
                case "24": return "GUJRAT";
                case "25": return "DAMAN and DIU";
                case "26": return "DADARA and NAGAR HAVELI";
                case "27": return "MAHARASHTRA";
                case "28": return "ANDHRA PRADESH";
                case "29": return "KARNATAKA";
                case "30": return "GOA";
                case "31": return "LAKSHADWEEP";
                case "32": return "KERALA";
                case "33": return "TAMIL NADU";
                case "34": return "PONDICHERRY";
                case "35": return "ANDAMAN and NICOBAR ISLANDS";
                case "36": return "TELANGANA";
                case "99": return "APO ADDRESS";
                default: return "UNKNOWN";
            }
        }
        public static string GetMaritalStatus(string code)
        {
            switch (code)
            {
                case "1":
                    return "Single";
                case "2":
                    return "Married";
                case "3":
                    return "Widow/Widower";
                case "4":
                    return "Divorced";
                default:
                    return "UNKNOWN";
            }
        }
        public static string GetEmploymentStatus(string empcode)
        {
            switch (empcode)
            {
                case "S":
                    return "Salaried";
                case "N":
                    return "Non-Salaried";
                case "E":
                    return "Self-employed";
                case "P":
                    return "Self-employed Professional";
                case "U":
                    return "Unemployed";
                default:
                    return "Unknown";
            }
        }
        public static string GetLoanAccountType(string accountCode)
        {
            switch (accountCode)
            {
                case "01": return "Auto Loan";
                case "02": return "Housing Loan";
                case "03": return "Property Loan";
                case "04": return "Loan Against Shares/Securities";
                case "05": return "Personal Loan";
                case "06": return "Consumer Loan";
                case "07": return "Gold Loan";
                case "08": return "Educational Loan";
                case "09": return "Loan to Professional";
                case "10": return "Credit Card";
                case "11": return "Leasing";
                case "12": return "Overdraft";
                case "13": return "Two-Wheeler Loan";
                case "14": return "Non-Funded Credit Facility";
                case "15": return "Loan Against Bank Deposits";
                case "16": return "Fleet Card";
                case "17": return "Commercial Vehicle Loan";
                case "18": return "Telco – Wireless";
                case "19": return "Telco – Broadband";
                case "20": return "Telco – Landline";
                case "23": return "GECL Secured";
                case "24": return "GECL Unsecured";
                case "31": return "Secured Credit Card";
                case "32": return "Used Car Loan";
                case "33": return "Construction Equipment Loan";
                case "34": return "Tractor Loan";
                case "35": return "Corporate Credit Card";
                case "36": return "Kisan Credit Card";
                case "37": return "Loan on Credit Card";
                case "38": return "PM Jan Dhan Yojana – Overdraft";
                case "39": return "Mudra Loan – Shishu / Kishor / Tarun";
                case "40": return "Microfinance – Business Loan";
                case "41": return "Microfinance – Personal Loan";
                case "42": return "Microfinance – Housing Loan";
                case "43": return "Microfinance – Others";
                case "44": return "PMAY – CLSS";
                case "45": return "P2P Personal Loan";
                case "46": return "P2P Auto Loan";
                case "47": return "P2P Education Loan";
                case "50": return "Business Loan – Secured";
                case "51": return "Business Loan – General";
                case "52": return "Business Loan – Priority Sector – Small Business";
                case "53": return "Business Loan – Priority Sector – Agriculture";
                case "54": return "Business Loan – Priority Sector – Others";
                case "55": return "Business Non-Funded Credit Facility – General";
                case "56": return "Business Non-Funded Credit Facility – Priority Sector – Small Business";
                case "57": return "Business Non-Funded Credit Facility – Priority Sector – Agriculture";
                case "58": return "Business Non-Funded Credit Facility – Priority Sector – Others";
                case "59": return "Business Loan Against Bank Deposits";
                case "60": return "Staff Loan";
                case "61": return "Business Loan – Unsecured";
                case "69": return "Short Term Personal Loan (Unsecured)";
                case "70": return "Priority Sector Gold Loan (Secured)";
                case "71": return "Temporary Overdraft (Unsecured)";
                case "00": return "Others";
                default: return "Unknown Account Type";
            }
        }
        public static string GetAccountStatus(string statusCode)
        {
            switch (statusCode)
            {
                case "00": return "No Suit Filed";
                case "11": return "Active";
                case "12": return "Closed";
                case "13": return "Closed";
                case "14": return "Closed";
                case "15": return "Closed";
                case "16": return "Closed";
                case "17": return "Closed";

                case "21": return "Active";
                case "22": return "Active";
                case "23": return "Active";
                case "24": return "Active";
                case "25": return "Active";

                case "30": return "Restructured";
                case "31": return "Restructured Loan (Govt. Mandated)";
                case "32": return "Settled";
                case "33": return "Post (WO) Settled";
                case "34": return "Account Sold";
                case "35": return "Written Off and Account Sold";
                case "36": return "Account Purchased";
                case "37": return "Account Purchased and Written Off";
                case "38": return "Account Purchased and Settled";
                case "39": return "Account Purchased and Restructured";
                case "40": return "Status Cleared";
                case "41": return "Restructured Loan";
                case "42": return "Restructured Loan (Govt. Mandated)";
                case "43": return "Written Off";
                case "44": return "Settled";
                case "45": return "Post (WO) Settled";
                case "46": return "Account Sold";
                case "47": return "Written Off and Account Sold";
                case "48": return "Account Purchased";
                case "49": return "Account Purchased and Written Off";
                case "50": return "Account Purchased and Settled";
                case "51": return "Account Purchased and Restructured";
                case "52": return "Status Cleared";

                case "53": return "Suit Filed";
                case "54": return "Suit Filed and Written Off";
                case "55": return "Suit Filed and Settled";
                case "56": return "Suit Filed and Post (WO) Settled";
                case "57": return "Suit Filed and Account Sold";
                case "58": return "Suit Filed and Written Off and Account Sold";
                case "59": return "Suit Filed and Account Purchased";
                case "60": return "Suit Filed and Account Purchased and Written Off";
                case "61": return "Suit Filed and Account Purchased and Settled";
                case "62": return "Suit Filed and Account Purchased and Restructured";
                case "63": return "Suit Filed and Status Cleared";

                case "64": return "Wilful Default and Restructured Loan";
                case "65": return "Wilful Default and Restructured Loan (Govt. Mandated)";
                case "66": return "Wilful Default and Settled";
                case "67": return "Wilful Default and Post (WO) Settled";
                case "68": return "Wilful Default and Account Sold";
                case "69": return "Wilful Default and Written Off and Account Sold";
                case "70": return "Wilful Default and Account Purchased";
                case "72": return "Wilful Default and Account Purchased and Written Off";
                case "73": return "Wilful Default and Account Purchased and Settled";
                case "74": return "Wilful Default and Account Purchased and Restructured";
                case "75": return "Wilful Default and Status Cleared";

                case "76": return "Suit Filed (Wilful Default) and Restructured";
                case "77": return "Suit Filed (Wilful Default) and Restructured Loan (Govt. Mandated)";
                case "79": return "Suit Filed (Wilful Default) and Settled";
                case "81": return "Suit Filed (Wilful Default) and Post (WO) Settled";
                case "85": return "Suit Filed (Wilful Default) and Account Sold";
                case "86": return "Suit Filed (Wilful Default) and Written Off and Account Sold";
                case "87": return "Suit Filed (Wilful Default) and Account Purchased";
                case "88": return "Suit Filed (Wilful Default) and Account Purchased and Written Off";
                case "89": return "Wilful Default";
                case "90": return "Suit Filed (Wilful Default) and Account Purchased and Restructured";
                case "91": return "Suit Filed (Wilful Default) and Status Cleared";
                case "93": return "Suit Filed (Wilful Default)";
                case "94": return "Suit Filed (Wilful Default) and Account Purchased and Settled";
                case "97": return "Suit Filed (Wilful Default) and Written Off";

                case "71": return "Active";
                case "78": return "Active";
                case "80": return "Active";
                case "82": return "Active";
                case "83": return "Active";
                case "84": return "Active";
                case "DEFAULTVALUE": return "Active";

                case "130": return "Restructured due to COVID-19";
                case "131": return "Restructured due to Natural Calamity";
                case "132": return "Post Write-Off Closed";
                case "133": return "Restructured & Closed";
                case "134": return "Auctioned & Settled";
                case "135": return "Repossessed & Settled";
                case "136": return "Guarantee Invoked";
                case "137": return "Entity Ceased while Account was Open";
                case "138": return "Entity Ceased while Account was Closed";

                default:
                    return "Unknown Account Status";
            }
        }
        public static string GetAccountHolderType(string holderTypeCode)
        {
            switch (holderTypeCode)
            {
                case "1": return "Individual";
                case "2": return "Joint";
                case "3": return "Authorized User";
                case "7": return "Guarantor";
                case "Z": return "Deceased";
                default: return "Unknown Holder Type";
            }
        }
        public static string GetInstitutionType(string institutionTypeCode)
        {
            switch (institutionTypeCode)
            {
                case "COB": return "Co-operative Bank";
                case "FOR": return "Foreign Bank";
                case "HFC": return "Housing Finance Company";
                case "NBF": return "Non-Banking Financial Institution";
                case "PUB": return "Public Sector Bank";
                case "PVT": return "Private Sector Bank";
                case "RRB": return "Regional Rural Bank";
                case "TEL": return "Telecom";
                case "SRC": return "Asset Reconstruction Company";
                case "MFI": return "Microfinance Institution";
                case "INS": return "Insurance Sector";
                case "CCS": return "Cooperative Credit Society";
                case "BRO": return "Brokerage Firm";
                case "CRA": return "Credit Rating Agency";
                case "OTH": return "Others";
                default: return "Unknown Institution Type";
            }
        }
        public static string GetWrittenOffSettledStatus(string statusCode)
        {
            switch (statusCode)
            {
                case "00": return "Restructured Loan";
                case "01": return "Restructured Loan (Govt. Mandated)";
                case "02": return "Written Off";
                case "03": return "Settled";
                case "04": return "Post (WO) Settled";
                case "05": return "Account Sold";
                case "06": return "Written Off and Account Sold";
                case "07": return "Account Purchased";
                case "08": return "Account Purchased and Written Off";
                case "09": return "Account Purchased and Settled";
                case "10": return "Account Purchased and Restructured";
                case "11": return "Restructured due to Natural Calamity";
                case "12": return "Restructured due to COVID-19";
                case "13": return "Post Write-Off Closed";
                case "14": return "Restructured & Closed";
                case "15": return "Auctioned & Settled";
                case "16": return "Repossessed & Settled";
                case "17": return "Guarantee Invoked";
                case "99": return "Clear Existing Status";
                default: return "Unknown Written-Off / Settled Status";
            }
        }
        public static string GetCollateralType(string collateralCode)
        {
            switch (collateralCode)
            {
                case "99": return "No Collateral";
                case "11": return "Property";
                case "12": return "Gold";
                case "13": return "Shares";
                case "14": return "Savings Account and Fixed Deposit";
                case "05": return "Multiple Securities";
                case "06": return "Others";
                default: return "Unknown Collateral Type";
            }
        }
        public static string GetPortfolioType(string portfolioTypeCode)
        {
            switch (portfolioTypeCode)
            {
                case "F": return "Microfinance";
                case "I": return "Instalment Loans";
                case "M": return "Mortgage Loan";
                case "L": return "Open Lines of Credit";
                case "R": return "Revolving Credit";
                case "S": return "Single Payment Loans";
                case "B": return "Banking";
                case "X": return "Leasing";
                default: return "Unknown Portfolio Type";
            }
        }
        public static string GetPaymentStatus(string paymentStatusCode)
        {
            switch (paymentStatusCode)
            {
                case "N":
                case "?":
                    return "Value Not Available";

                case "0":
                    return "0–29 Days Past Due";
                case "1":
                    return "30–59 Days Past Due";
                case "2":
                    return "60–89 Days Past Due";
                case "3":
                    return "90–119 Days Past Due";
                case "4":
                    return "120–149 Days Past Due";
                case "5":
                    return "150–179 Days Past Due";
                case "6":
                    return "180 or More Days Past Due";

                case "S":
                    return "Standard Asset";
                case "B":
                    return "Substandard Asset";
                case "D":
                    return "Doubtful Asset";
                case "M":
                    return "Special Mention Account";
                case "L":
                    return "Loss Asset";

                default:
                    return "Unknown Payment Status";
            }
        }

    }
}
