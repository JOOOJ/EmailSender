using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;

namespace EMailSender
{
    public class EmailSender
    {
        private string m_SMTPServer;
        public EmailSender(string SMTPServerIP)
        {
            m_SMTPServer = SMTPServerIP;
        }

        public void Send(string From, List<string> To, List<string> CC, string Subject, string Body)
        {
            MailMessage mail = SendPrepare(From, To, CC, Subject, Body);
            mail.IsBodyHtml = true;
            StartSend(mail);
        }

        public void Send(string From, List<string> To, List<string> CC, string Subject, string Body, List<string> Bcc)
        {
            MailMessage mail = SendPrepare(From, To, CC, Subject, Body);
            if (Bcc != null)
            {
                foreach (string item in Bcc)
                {
                    mail.Bcc.Add(new MailAddress(item));
                }
            }
            mail.IsBodyHtml = true;
            StartSend(mail);
        }

        public void Send(string From, List<string> To, List<string> CC, string Subject, string Body, List<string> Bcc, List<string> attachFiles)
        {
            using (MailMessage mail = SendPrepare(From, To, CC, Subject, Body))
            {
                foreach (string item in attachFiles)
                {
                    if (!File.Exists(item))
                    {
                        throw new IOException("Attchements don't exist!");
                    }
                    mail.Attachments.Add(new Attachment(item));
                }
                if (Bcc != null)
                {
                    foreach (string item in Bcc)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
                mail.IsBodyHtml = true;
                StartSend(mail);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="CC"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="Bcc"></param>
        /// <param name="attachFiles"></param>
        /// <param name="inlineImageFullPath"></param>
        /// <param name="imageType">ONLY support Gif,Jpeg,Tiff</param>
        public void Send(string From, List<string> To, List<string> CC, string Subject, string Body, List<string> Bcc, List<string> attachFiles, string inlineImageFullPath, string imageType)
        {
            using (MailMessage mail = SendPrepare(From, To, CC, Subject, Body))
            {
                LinkedResource inline = null;
                string temp = imageType.ToLower();
                if (temp == "gif")
                {
                    inline = new LinkedResource(inlineImageFullPath, MediaTypeNames.Image.Gif);
                }
                else if (temp == "jpeg")
                {
                    inline = new LinkedResource(inlineImageFullPath, MediaTypeNames.Image.Jpeg);
                    Console.WriteLine("jpeg");
                }
                else if (temp == "tiff")
                {
                    inline = new LinkedResource(inlineImageFullPath, MediaTypeNames.Image.Tiff);
                }
                else
                {
                    throw new ArgumentException("Image type only support Gif,Jpeg and Tiff! Please choose correct image type.");
                }
                inline.ContentId = Guid.NewGuid().ToString();
                string htmlBody = string.Format(@"<html><body><img src=""cid:{0}"" /></body></html>", inlineImageFullPath);
                //Console.WriteLine(htmlBody);
                mail.Body = string.Format(Body, inline.ContentId);
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(Body, null, MediaTypeNames.Text.Html);
                avHtml.LinkedResources.Add(inline);
                Attachment att = new Attachment(inlineImageFullPath);
                att.ContentDisposition.Inline = true;
                //mail.AlternateViews.Add(avHtml);
                mail.Attachments.Add(att);
                foreach (string item in attachFiles)
                {
                    if (!File.Exists(item))
                    {
                        throw new IOException("Attchements don't exist!");
                    }
                    mail.Attachments.Add(new Attachment(item));
                }
                if (Bcc != null)
                {
                    foreach (string item in Bcc)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
                mail.Body = string.Format(Body, inline.ContentId);
                mail.IsBodyHtml = true;
                StartSend(mail);
            }
        }

        private void StartSend(MailMessage mail)
        {
            using (SmtpClient client = new SmtpClient(m_SMTPServer))
            {
                try
                {
                    client.Send(mail);
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    throw ex;
                }
            }

        }

        private static MailMessage SendPrepare(string From, List<string> To, List<string> CC, string Subject, string Body)
        {
            if (string.IsNullOrEmpty(From) || To == null || To.Count == 0 || string.IsNullOrEmpty(Subject))
            {
                throw new ArgumentException("Mail From or To is empty, please input them", "From,To,Subject", new Exception());
            }
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(From);
            foreach (string item in To)
            {
                mail.To.Add(new MailAddress(item));
            }
            if (CC != null)
            {
                foreach (string item in CC)
                {
                    mail.CC.Add(new MailAddress(item));
                }
            }
            mail.Subject = Subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = Body;
            return mail;
        }
    }
}
