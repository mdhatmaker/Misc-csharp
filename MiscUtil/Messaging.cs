using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;

namespace TT.Hatmaker
{
    public enum Carrier { Cingular = 0, Nextel = 1, Sprint = 3, TMobile = 4, Verizon = 5, VirginMobile = 6, ATT = 7 }

    public class Messaging
    {
        private void ShowMailMessage(string msg, bool b)
        {
            Console.WriteLine(msg);
        }

        private void Clear()
        {
        }

        public string ConvertCellularNumberToEmail(Carrier carrier, string phoneNumber)
        {
            string email = null;

            switch (carrier)
            {
                case Carrier.Cingular:
                    email = phoneNumber + "@cingularme.com";
                    break;
                case Carrier.Nextel:
                    email = phoneNumber + "@messaging.nextel.com";
                    break;
                case Carrier.Sprint:
                    email = phoneNumber + "@messaging.sprintpcs.com";
                    break;
                case Carrier.TMobile:
                    email = phoneNumber + "@tmomail.net";
                    break;
                case Carrier.Verizon:
                    email = phoneNumber + "@vtext.com";
                    break;
                case Carrier.VirginMobile:
                    email = phoneNumber + "@vmobl.com";
                    break;
                case Carrier.ATT:
                    email = phoneNumber + "@txt.att.net";
                    break;
            }
            return email;
        }

        public void SendMail(string host, int port, string userName, string pswd, string fromAddress, string toAddress, string body, string subject, bool sslEnabled)
        {
            MailMessage msg = new MailMessage(new MailAddress(fromAddress), new MailAddress(toAddress));    //  Create a MailMessage object with a from and to address
            msg.Subject = subject;  //  Add your subject
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = body;    //  Add the body of your message
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.IsBodyHtml = false; //  Does the body contain html

            SmtpClient client = new SmtpClient(host, port); //  Create an instance of SmtpClient with your smtp host and port
            client.Credentials = new NetworkCredential(userName, pswd); //  Assign your username and password to connect to gmail
            client.EnableSsl = sslEnabled;  //  Enable SSL

            try
            {
                client.Send(msg);   //  Try to send your message
                ShowMailMessage("Your message was sent successfully.", false);  //  A method to update a ui element with a message
                Clear();
            }
            catch (SmtpException ex)
            {
                ShowMailMessage(string.Format("There was an error sending you message. {0}", ex.Message), true);
            }
        }

    }   // class Messaging
}   // namespace MiscUtil
