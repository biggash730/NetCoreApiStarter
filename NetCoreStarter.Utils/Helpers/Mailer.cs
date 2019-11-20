using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace NetCoreStarter.Utils
{
    public class Mailer
    {
        public bool Success { get; private set; }
        public string Response { get; private set; }

        private void ResultHandler(string message, bool success = true)
        {
            Success = success;
            Response = message;
        }

        public void SendViaMg(string receipient, string subject, string message)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api",
                    "key-107377cc0b03ac75f9b404b8ec526285")
            };
            var request = new RestRequest();
            //todo: change the domain
            request.AddParameter("domain", "mg.devnestsystems.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Ghana Legality Assurance Portal <mailgun@ghlegalitypotal.com>");
            request.AddParameter("to", receipient);
            request.AddParameter("subject", subject);
            request.AddParameter("text", message);
            request.Method = Method.POST;
            var result = client.Execute(request);

            ResultHandler(result.ErrorMessage, (result.StatusCode == HttpStatusCode.OK));
        }
    }
}
