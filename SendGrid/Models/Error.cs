using System;
using MongoDB.Bson;

namespace SendGrid.Models
{
    public class Error
    {
        public Error() {}

        public Error(Exception exception)
        {
            Date = DateTime.Now;
            Message = string.Format("{0}: {1}", exception.GetType().Name, exception.Message);
            StackTrace = exception.StackTrace;
        }

        public BsonObjectId Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Date { get; set; }
    }
}