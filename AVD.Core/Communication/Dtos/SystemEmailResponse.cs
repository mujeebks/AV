namespace AVD.Core.Communication.Dtos
{
    using System.Collections.Generic;

    public class SystemEmailResponse
    {
        public SystemEmailResponse()
        {
            this.ValidationMessages = new List<ValidationMessage>();
            this.FailureMessages = new List<FailureMessage>();
            this.IsSuccessful = false;
        }

        public List<ValidationMessage> ValidationMessages { get; set; }
        public List<FailureMessage> FailureMessages { get; private set; }
        public bool IsSuccessful { get; set; }
    }
}