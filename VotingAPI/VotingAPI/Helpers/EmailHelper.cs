using System.Security.Cryptography;

namespace VotingAPI.Helpers
{
    public static class EmailHelper
    {
        public static string GetOtp()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        public static string GetBody(string userName, string otp, int validMinutes = 10)
        {
            return $@"
<!DOCTYPE html> 
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=viewport content=""width=device-width, initial-scale=1.0"">
<title>Verification Code</title>
</head>
    <body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f9f9f9; color: #333333;"">    
        <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); border: 1px solid #e1e1e1;"">        
            <tr>            
                <td style=""padding: 40px 30px;"">                
                    <!-- Header / App Name -->                
                    <h2 style=""margin-top: 0; color: #1a1a1a; font-size: 22px; font-weight: 600; border-bottom: 1px solid #eeeeee; padding-bottom: 15px;"">                    
                        Blockchain Voting System                
                    </h2>                                
                    
                    <!-- Greeting -->                
                    <p style=""font-size: 16px; line-height: 1.5; color: #4a4a4a; margin-top: 20px;"">                    
                        Hello {userName},                
                    </p>                                
                    
                    <p style=""font-size: 16px; line-height: 1.5; color: #4a4a4a;"">                    
                        Thank you for authenticating with <strong>Blockchain Voting System</strong>. To complete your sign-in or verification process, please use the following one-time password (OTP):                
                    </p>                                
                    
                    <!-- OTP Display Box -->                
                    <div style=""text-align: center; margin: 30px 0;"">                    
                        <div style=""display: inline-block; padding: 14px 32px; background-color: #f1f5f9; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 28px; font-weight: 700; letter-spacing: 4px; color: #0f172a;"">                        
                            {otp}                    
                        </div>                
                    </div>                                
                    
                    <!-- Divider -->                
                    <hr style=""border: 0; border-top: 1px solid #eeeeee; margin: 30px 0;"">                                
                    
                    <!-- Important Information section -->                
                    <div style=""background-color: #fffbeb; border-left: 4px solid #f59e0b; padding: 15px; border-radius: 4px;"">                    
                        <h4 style=""margin: 0 0 8px 0; color: #b45309; font-size: 14px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px;"">                        
                            Important Information                    </h4>                    
                        <p style=""margin: 0; font-size: 14px; line-height: 1.4; color: #78350f;"">                        
                            This code is valid for <strong>{validMinutes} minutes</strong> from the time it was requested.                    
                        </p>                
                    </div>                            
                </td>        
            </tr>        
            <!-- Footer -->        
            <tr>            
                <td style=""padding: 0 30px 30px 30px; font-size: 12px; color: #94a3b8; text-align: center;"">                
                    <p style=""margin: 0;"">This is an automated security notification. Please do not reply to this email.</p>           
                </td>        
            </tr>    
        </table>
    </body>
    </html>";
        }
    }
}