using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common
{
    public interface ICurrentUser
    {
       int Campus { get; }
       bool MasterAccess { get; }
    }
}
