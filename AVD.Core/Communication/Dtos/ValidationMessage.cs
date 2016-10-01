namespace AVD.Core.Communication.Dtos
{
    public class ValidationMessage
    {
        public ValidationMessage(ValidationMessageType type, EmailDtoField targetField)
        {
            this.Type = type;
            this.TargetField = targetField;
        }

        public EmailDtoField TargetField { get; set; }
        public ValidationMessageType Type { get; set; }
    }
}