using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleRoyalServer.Common
{
    public enum ErrorCode : byte
    {
        Success = 0,
        InvalidParametrs,
        NameIsExist,
        RequestNotImplemented
    }
}
