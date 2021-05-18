using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Pdf
{
    public class EmailService
    {
        public bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                memoryStream.Position = 0;
                ContentType contentType =new ContentType(MediaTypeNames.Application.Pdf);
                Attachment attach = new Attachment(memoryStream, contentType);
                attach.ContentDisposition.FileName = $"{fileName}.pdf";

                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();

                mailMessage.From = new MailAddress("dumymailadress44@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Test";
                mailMessage.Body = "Test test test ";
                mailMessage.IsBodyHtml = true;
                mailMessage.Attachments.Add(attach);

                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("dumymailadress44@gmail.com", "*********");
                smtpClient.Send(mailMessage);
                Console.WriteLine("Mail has been sended.");

                memoryStream.Close();
                memoryStream.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");
                return false;
            }

        }

        
    }
}