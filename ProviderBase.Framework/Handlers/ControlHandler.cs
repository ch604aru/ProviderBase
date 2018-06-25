using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System;

namespace ProviderBase.Framework.Handlers
{
    public class ControlHandler : BaseHandler
    {
        public void Login()
        {
            string username = ProviderBase.Framework.Utility.GetFormValue<string>("User_Username", "");
            string password = ProviderBase.Framework.Utility.GetFormValue<string>("User_Password", "");

            if (string.IsNullOrEmpty(username))
            {
                this.AjaxResult.Message = "Please enter a username";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
            else if (string.IsNullOrEmpty(password))
            {
                this.AjaxResult.Message = "Please enter a password";
                this.AjaxResult.Status = AjaxResultStatus.Failed;
            }
            else
            {
                User loggedInUser = null;

                loggedInUser = ProviderBase.Data.Providers.DataProvider.SelectSingle<User>(new User()
                {
                    Username = username
                }, this.Website.WebsiteConnection.ConnectionString);

                if (loggedInUser?.UserID > 0)
                {
                    bool hashCheck = false;

                    hashCheck = ProviderBase.Data.Entities.Encryption.HashCheck(password, loggedInUser.Encrypted);

                    if (hashCheck)
                    {
                        string token = "";
                        string hmac = "";

                        loggedInUser.LastLogin = DateTime.Now;

                        ProviderBase.Data.Providers.DataProvider.Update<User>(loggedInUser, this.Website.WebsiteConnection.ConnectionString);

                        token = loggedInUser.UserID.ToString() + "|" + (long)(DateTime.Now.AddMinutes(20) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                        hmac = ProviderBase.Data.Entities.Encryption.HashHMAC(token);
                        token = ProviderBase.Data.Entities.Encryption.EncryptString(token);

                        this.Context.Response.Cookies["ProviderBaseAuthentication"]["Hmac"] = hmac;
                        this.Context.Response.Cookies["ProviderBaseAuthentication"]["Token"] = token;

                        this.AjaxResult.Message = "Login success";
                        this.AjaxResult.Status = AjaxResultStatus.Success;
                    }
                    else
                    {
                        this.AjaxResult.Message = "Bad username or password";
                        this.AjaxResult.Status = AjaxResultStatus.Failed;
                    }
                }
                else
                {
                    this.AjaxResult.Message = "Bad username or password";
                    this.AjaxResult.Status = AjaxResultStatus.Failed;
                }
            }
        }

        public void Logout()
        {
            this.Context.Response.Cookies["ProviderBaseAuthentication"]["Hmac"] = "";
            this.Context.Response.Cookies["ProviderBaseAuthentication"]["Token"] = "";

            this.AjaxResult.Message = "Logout success";
            this.AjaxResult.Status = AjaxResultStatus.Success;
        }
    }
}
