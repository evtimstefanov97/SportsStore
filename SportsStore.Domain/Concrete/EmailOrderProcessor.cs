using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "evtim_stefanov@yahoo.com";
        public string MailFromAddress = "evtim_stefanov@yahoo.com";
        public bool UseSsl = true;
        public string Username = "evtim_stefanov@yahoo.com";
        public string Password = "qwertykey123A";
        public string ServerName = "smtp.mail.yahoo.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\SportsStore_emails";
    }
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingInfoDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                
                StringBuilder body = new StringBuilder();
                body.AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");
                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }
                body.AppendFormat("Total order value: {0:c}", cart.TotalSum())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingInfoDetails.Name)
                    .AppendLine(shippingInfoDetails.Line1)
                    .AppendLine(shippingInfoDetails.Line2 ?? "")
                    .AppendLine(shippingInfoDetails.Line3 ?? "")
                    .AppendLine(shippingInfoDetails.City)
                    .AppendLine(shippingInfoDetails.State ?? "")
                    .AppendLine(shippingInfoDetails.Country)
                    .AppendLine(shippingInfoDetails.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingInfoDetails.GiftWrap ? "Yes" : "No");
                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress,emailSettings.MailToAddress,"Submitted",body.ToString());
               
                smtpClient.Send(mailMessage);
            }
        }
    }
}
