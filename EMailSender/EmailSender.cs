﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace EMailSender
{
    public class EmailSender
    {
        private string m_SMTPServer;
        public EmailSender(string SMTPServerIP)
        {
            m_SMTPServer = SMTPServerIP;
        }

        public void Send(string From,List<string> To, List<string> CC, string Subject,string Body)
        {
            MailMessage mail = SendPrepare(From, To, CC,Subject,Body);
            StartSend(mail);
        }

        public void Send(string From, List<string> To, List<string> CC, string Subject, string Body,List<string> Bcc)
        {
            MailMessage mail = SendPrepare(From, To, CC, Subject, Body);
            if(Bcc!=null)
            {
                foreach (string item in Bcc)
                {
                    mail.Bcc.Add(new MailAddress(item));
                }
            }
            StartSend(mail);
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
                    //foreach (var innerEx in ex.InnerExceptions)
                    //{
                    //    if (innerEx.StatusCode == SmtpStatusCode.MailboxBusy || innerEx.StatusCode == SmtpStatusCode.MailboxUnavailable)
                    //    {
                    //        client.Send(mail);
                    //    }
                    //}
                    throw ex;
                }
            }         
            
        }

        private static MailMessage SendPrepare(string From, List<string> To,List<string> CC, string Subject, string Body)
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
