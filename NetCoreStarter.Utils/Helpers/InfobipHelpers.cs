using System.Collections.Generic;

namespace NetCoreStarter.Utils
{
    public class InfobipHelpers
    {
        public class InfobipSmsMessage
        {
            public string from { get; set; }
            public string text { get; set; }
            public string to { get; set; }
        }
        public class InfobipSmsResponse
        {
            public string bulkId { get; set; }
            public List<InfobipSmsResponseMessage> messages { get; set; }
        }
        public class InfobipSmsResponseMessage
        {
            public InfobipMessageStatus status { get; set; }
            public string to { get; set; }
            public string smsCount { get; set; }
            public string messageId { get; set; }
        }
        public class InfobipMessageStatus
        {
            public long id { get; set; }
            public long groupId { get; set; }
            public string groupName { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }
    }
}
