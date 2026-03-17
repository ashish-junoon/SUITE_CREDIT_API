using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JC.CIC.Model.TransUnion
{
    public class HybridModel
    {
        public class HybridRequest
        {
            public string Data { get; set; }
        }

        public class HybridEncryptedPacket
        {
            public string EncryptedKey { get; set; }
            public string EncryptedData { get; set; }
        }
    }
}
