using System.Web;
using System.IO;
using System;
using System.Net;
using Newtonsoft.Json;

namespace ProviderBase.Web
{
    public static class Utility
    {
        public static string GetResourcePath(string fileName)
        {
            HttpContext context = HttpContext.Current;

            return context.Server.MapPath("Resource/Html/" + fileName);
        }

        public static string GetResourceHtml(string fileName)
        {
            HttpContext context = HttpContext.Current;
            string filePath = GetResourcePath(fileName);
            string html = "";

            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    html = sr.ReadToEnd();
                }
            }

            return html;
        }

        public static string GetReturnResourceHtml(string fileName)
        {
            if (fileName.EndsWith(".html") || fileName.EndsWith(".htm"))
            {
                return GetResourceHtml(fileName);
            }
            else
            {
                return fileName;
            }
        }

        public static void SaveResourceHtml(string fileName, string html)
        {
            HttpContext context = HttpContext.Current;
            string filePath = GetResourcePath(fileName);

            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                sw.Write(html);
            }
        }

        public static string FormatExceptionForHtml(HttpContext context, Exception exception)
        {
            string html = "";

            if (context.Request.IsLocal == false)
            {
                html = $"<p>Uh oh<p>";
                html = $"<p>We have encountered an error<p>";
                html = $"<p>We have sent this to our technical team who will take a look asap<p>";
                html = $"<p>In the meantime you can refresh the page and try again or come back a bit later<p>";
                html = $"<p>Sorry about that<p>";
            }
            else
            {

                html = $"<p>URL: {context.Request.Url.ToString()}<p>";

                html += "<br/><p>------------------------------<p/><br/>";

                if (exception.InnerException != null)
                {
                    if (exception.InnerException.StackTrace != null)
                    {
                        html += $"<p>Stacktrace:</p>";
                        html += $"<p>{exception.InnerException.StackTrace.ToString()}</p>";

                        html += "<br/><p>------------------------------<p/><br/>";
                    }

                    html += $"<p>Error Message:</p>";
                    html += $"<p>{exception.InnerException.Message}</p>";
                }
                else
                {
                    if (exception.StackTrace != null)
                    {
                        html += $"<p>Stacktrace:</p>";
                        html += $"<p>{exception.StackTrace.ToString()}</p>";

                        html += "<br/><p>------------------------------<p/><br/>";
                    }

                    html += $"<p>Error Message:</p>";
                    html += $"<p>{exception.Message}</p>";
                }

                html += "<br/><p>------------------------------<p/><br/>";
                html += "<p>Querystring:</p>";

                for (int i = 0; i < context.Request.QueryString.Count; i++)
                {
                    html += (i > 0) ? "<br/>" : "";
                    html += $"<p>{context.Request.QueryString.Keys[i].ToString()} --- {context.Request.QueryString[i].ToString()}</p>";
                }

                html += "<br/><p>------------------------------<p/><br/>";
                html += "<p>FormValues:</p>";

                for (int i = 0; i < context.Request.Form.Count; i++)
                {
                    string formKeyAndValue = "";

                    formKeyAndValue += (i > 0) ? "<br/>" : "";

                    try
                    {
                        formKeyAndValue += $"<p>{context.Request.Form.Keys[i].ToString()}";
                    }
                    catch
                    {
                        formKeyAndValue += "<p><error>";
                    }

                    formKeyAndValue += " --- ";

                    try
                    {
                        formKeyAndValue += $"{context.Request.Form[i].ToString()}</p>";
                    }
                    catch
                    {
                        formKeyAndValue += "<error></p>";
                    }

                    html += formKeyAndValue;
                }

                html += "<br/><p>------------------------------<p/><br/>";
            }

            return html;
        }

        public static T WebRequestGet<T>(string requestURL) where T : new()
        {
            string responseHtml = "";
            HttpWebRequest request = null;
            T returnObject = new T();

            request = (HttpWebRequest)WebRequest.Create(requestURL);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseHtml = reader.ReadToEnd();
            }

            returnObject = JsonConvert.DeserializeObject<T>(responseHtml);

            returnObject = (returnObject == null) ? new T() : returnObject;

            return returnObject;
        }
    }
}
