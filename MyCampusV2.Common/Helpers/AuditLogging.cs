using MyCampusV2.Models.V2.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCampusV2.Common.Helpers
{
    public class AuditLogging
    {
        public eventLoggingEntity fillEventLogging(int userId, int formId, string source, string category, bool logLevel, string message, DateTime logDateTime)
        {
            eventLoggingEntity eventLogging = new eventLoggingEntity();
            eventLogging.User_ID = userId;
            eventLogging.Form_ID = formId;
            eventLogging.Source = source;
            eventLogging.Category = category;
            eventLogging.Log_Level = logLevel;
            eventLogging.Message = message;
            eventLogging.Log_Date_Time = logDateTime;
            return eventLogging;
        }
    }
}
