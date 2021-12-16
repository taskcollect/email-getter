/*
xxori - Patrick
Class representing an email object, instantiated for each email received from api
*/

namespace exchangeapi.Models {
    public class Email {
        public string Id { get; set; }
        public string From { get; set; }
        public bool isRead { get; set; }
        public string Subject { get; set; }
        public long TimeSent { get; set; }
    }
}