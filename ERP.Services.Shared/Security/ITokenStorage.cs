using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Shared.Security;
public interface ITokenStorage
{
    void Set(JwtDto jwt);
    JwtDto Get();
}