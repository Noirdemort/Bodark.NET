using System;
using System.IO;
using System.Reflection;
using EasyHttp;
using EasyHttp.Http;

namespace Bodark
{
    public class NetUtils
    {
        public NetUtils()
        {

        }

        public static string Get(string url, string contentType, dynamic parameters)
        {
            var http = new HttpClient();
            http.Request.Accept = contentType;
            var response =  http.Get(url, parameters);
            return response.RawText;
        }

        public static string GetFile(string url, string contentType, dynamic parameters, string  fileName)
        {
            var httpClient = new HttpClient();
            var filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            var response = httpClient.GetAsFile(url, filename);
            return response.StatusCode.ToString();
        }

        public static int Post(string url, string contentType, dynamic parameters)
        {
            var http = new HttpClient();
            var response = http.Post(url, parameters, contentType);
            return (int)response.StatusCode;
        }

        public static string PostFile(string url, string contentType, dynamic parameters, string fileName)
        {
            var httpClient = new HttpClient();
            var response = httpClient.PutFile( url, fileName, contentType);
            return response.DynamicBody;
        }
    }
}
