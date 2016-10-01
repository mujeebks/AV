namespace AVD.Core.Communication.Dtos
{
    public enum ValidationMessageType
    {
        Unknown = 0,
        MissingEmailField,
        InvalidEmailField,
        MissingConfiguration,
        InvalidConfiguration,
        MissingDbValue,
        InvalidDbValue
    }
}