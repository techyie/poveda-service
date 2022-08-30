using MyCampusV2.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.DAL.Helpers
{
    public class Response
    {
        public ResultModel CreateResponse(string Code, string Message, bool isSuccess)
        {
            ResultModel result = new ResultModel();
            result.resultCode = Code;
            result.resultMessage = Message;
            result.isSuccess = isSuccess;
            return result;
        }
    }
}
