using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Mail;

namespace collageProject.Services
{
    public class EmailOtpService
    {
        private readonly IConfiguration _config;
        public EmailOtpService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateOTP(int length = 6)
        {
            var random = new Random();
            var otp = "";

            for (int i = 0; i < length; i++)
                otp += random.Next(0, 10); // 0-9

            return otp;
        }

        public async Task<bool> sendOtpEmail(string EmailId,string otp,string username)
        {
            try {
                var settings = _config.GetSection("EmailServiceSettings");
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(settings["SenderEmail"]);
                mailMessage.To.Add(EmailId);
                mailMessage.Subject = "OTP Verification";
                mailMessage.Body = SendOtpEmailBody(otp, username);
                mailMessage.IsBodyHtml = true;
                var smtp = new SmtpClient(settings["smtpServer"])
                {
                    Port = int.Parse(settings["Port"]),
                    Credentials = new NetworkCredential(
                    settings["SenderEmail"],
                    settings["Password"]
                    ),
                    EnableSsl = true
                };
                await smtp.SendMailAsync(mailMessage);
                DateTime expiryTime = DateTime.Now.AddMinutes(5);
                return true;
            }
            
            catch (Exception ex)
            {
                return false;
            }
        }
        private string SendOtpEmailBody(string otp, string username)
        {
            int currentYear = DateTime.Now.Year;
            return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>

                    <style>
                    /* Mobile Responsive */
                    @media only screen and (max-width: 600px) {{
                     .container {{
                        width: 100% !important;
                        margin: 0 !important;
                      }}

                      .content {{
                        padding: 20px !important;
                      }}

                      .otp-box {{
                        font-size: 26px !important;
                        letter-spacing: 3px !important;
                        padding: 10px 20px !important;
                      }}

                      .header {{
                        font-size: 20px !important;
                        padding: 15px !important;
                      }}

                      .text {{
                        font-size: 15px !important;
                      }}
                    }}
                    </style>
                    </head>

                    <body style='margin:0; padding:0; background:#f4f6f8; font-family:Arial, sans-serif;'>

                    <table width='100%' cellpadding='0' cellspacing='0' role='presentation'>
                    <tr>
                    <td align='center'>

                    <!-- Main Container -->
                    <table class='container' width='600' cellpadding='0' cellspacing='0'
                    style='max-width:600px; width:100%; background:#ffffff;
                    border-radius:8px; margin:20px; box-shadow:0 0 8px rgba(0,0,0,0.1);'>

                    <!-- Header -->
                    <tr>
                    <td class='header'
                    style='background:#2563eb; color:#ffffff; text-align:center;
                    font-size:24px; padding:20px; font-weight:bold;'>

                    Employee Leave Management System

                    </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                    <td class='content'
                    style='padding:30px; color:#333333; font-size:16px; line-height:1.6;'>

                    <p class='text'>Hello <strong>{username}</strong>,</p>

                    <p class='text'>
                    We received a request to verify your account.
                    Please use the OTP below:
                    </p>

                    <!-- OTP -->
                    <div style='text-align:center; margin:30px 0;'>

                    <span class='otp-box'
                    style='display:inline-block;
                    font-size:32px;
                    font-weight:bold;
                    letter-spacing:5px;
                    color:#2563eb;
                    background:#eef2ff;
                    padding:12px 28px;
                    border-radius:6px;'>

                    {otp}

                    </span>

                    </div>

                    <p class='text'>
                    This OTP is valid for <strong>5 minutes</strong>.
                    </p>

                    <p class='text'>
                    If you did not request this, please ignore this email.
                    </p>

                    <br/>

                    <p class='text'>
                    Regards,<br/>
                    <strong>ELMS Support Team</strong>
                    </p>

                    </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                    <td
                    style='background:#f1f5f9;
                    text-align:center;
                    font-size:13px;
                    color:#666666;
                    padding:15px;'>

                    © {DateTime.Now.Year} Employee Leave Management System  
                    All Rights Reserved

                    </td>
                    </tr>

                    </table>

                    </td>
                    </tr>
                    </table>

                    </body>
                    </html>";
        }

    }

 }

