using System.Collections.Generic;

namespace AVD.Core.Communication.Dtos
{
    public class EmailResponse
    {
        public EmailResponse()
        {
            this.ValidationMessages = new List<ValidationMessage>();
            this.FailureMessages = new List<FailureMessage>();
            this.IsSuccessful = false;
            this.EmailId = 0;
        }

        public List<ValidationMessage> ValidationMessages { get; set; }
        public List<FailureMessage> FailureMessages { get; private set; }
        public bool IsSuccessful { get; set; }
        public int EmailId { get; set; }
    }
}