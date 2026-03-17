using JC.TransUnion.Cibil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JC.TransUnion.Cibil.Interface
{
    public interface ICibilTokenService
    {
        Task<string> GetTokenAsync(string BasePath);
    }
}
