using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using AVD.DataAccessLayer.Enums;

namespace AVD.Core.Communication.Dtos
{
    public class EmailAddressDto
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }
    }

    public class EmailAttachmentDto
    {
        public string FileName { get; set; }
        public string EmailContentType { get; set; }
        public MemoryStream MemoryStream { get; set; }
        // save the attachment if it's unique, we don't need to continually save static ones.
        public bool isStatic { get; set; }
    }

    public class AttachmentContentType
    {
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is not
            //     interpreted.
            public const string Octet = "application/octet-stream";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in
            //     Portable Document Format (PDF).
            public const string Pdf = "application/pdf";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in
            //     Rich Text Format (RTF).
            public const string Rtf = "application/rtf";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a SOAP
            //     document.
            public const string Soap = "application/soap+xml";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is compressed.
            public const string Zip = "application/zip";
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Graphics
            //     Interchange Format (GIF).
            public const string Gif = "image/gif";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Joint
            //     Photographic Experts Group (JPEG) format.
            public const string Jpeg = "image/jpeg";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Tagged
            //     Image File Format (TIFF).
            public const string Tiff = "image/tiff";
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in HTML format.
            public const string Html = "text/html";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in plain text
            //     format.
            public const string Plain = "text/plain";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in Rich Text
            //     Format (RTF).
            public const string RichText = "text/richtext";
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in XML format.
            public const string Xml = "text/xml";
    }

    public class EmailRequestDto 
    {
        public EmailRequestDto()
        {
            //default Values
            this.BodyEncoding = Encoding.ASCII;
            this.BodyTransferEncoding = TransferEncoding.Unknown;
            this.Attachments = new List<EmailAttachmentDto>();
        }

        public EmailAddressDto From { get; set; }
        public List<EmailAddressDto> To { get; set; }
        public List<EmailAddressDto> CC { get; set; }
        public List<EmailAddressDto> Bcc { get; set; }
        public List<EmailAddressDto> ReplyToList { get; set; }
        public string Subject { get; set; }
        public Encoding SubjectEncoding { get; set; }
        public string Body { get; set; }
        public Encoding BodyEncoding { get; set; }
        public TransferEncoding BodyTransferEncoding { get; set; }
        public string ReplyTo { get; set; }
        public string BodyType { get; set; }
        public List<EmailAttachmentDto> Attachments { get; set; }
        public bool IsBodyHtml { get; set; }

        public EmailTypes EmailType { get; set; }
        public int QuoteId { get; set; }
        public bool IsResending { get; set; }
        public int AgentId { get; set; }
        public int? InvoiceId { get; set; }
        public string Url { get; set; }
        public string EmailIdentifier { get; set; }
    }
}
