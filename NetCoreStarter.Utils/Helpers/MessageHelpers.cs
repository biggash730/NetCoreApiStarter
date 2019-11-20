namespace NetCoreStarter.Utils
{
    public static class MessageHelpers
    {
        public static string GetContentType(string ext)
        {

            switch (ext)
            {
                case "pdf":
                    return "application/pdf";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "jpeg":
                    return "image/jpeg";
                case "png":
                    return "image/jpeg";
                case "jpg":
                    return "image/png";
                default:
                    return "application/pdf";

            }
        }

        //public static void ProcessEmail(long id)
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        var msg = db.Messages.First(x => x.Id == id);
        //        var user = db.Users.First(x => x.UserName == msg.CreatedBy);

        //        //send the email messages
        //        var templateFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempFiles");
        //        var client = new RestClient
        //        {
        //            BaseUrl = new Uri("https://api.mailgun.net/v3"),
        //            Authenticator = new HttpBasicAuthenticator("api",
        //                "key-2a3fec9de587928bde93299c20e1e376")
        //        };
        //        var request = new RestRequest();
        //        //request.
        //        request.AddParameter("domain",
        //            "blumamessage.6onelabs.com", ParameterType.UrlSegment);
        //        request.Resource = "{domain}/messages";
        //        request.AddParameter("from", SetupConfig.Setting.SmsMessageService.SenderId + " <mailgun@blumamessage.6onelabs.com>");
        //        request.AddParameter("to", msg.Receipient);
        //        request.AddParameter("subject", msg.Subject);
        //        request.AddParameter("html", msg.Content);
        //        request.AddParameter("text", "Bluma Messaging Service");

        //        //build the attachment
        //        if (msg.FileBase64String != null)
        //        {
        //            var fileName = msg.FileName + "." + msg.FileExtension;
        //            var fn = Path.Combine(templateFolder, fileName);
        //            File.Delete(fn);
        //            //var contentType = GetContentType(msg.FileExtension);
        //            File.WriteAllBytes(fn, Convert.FromBase64String(msg.FileBase64String));
        //            request.AddFile("attachment", fn);
        //        }
        //        request.Method = Method.POST;
        //        var res = client.Execute(request);

        //        if (res.StatusCode == HttpStatusCode.OK)
        //        {
        //            msg.Status = MessageStatus.Sent;
        //        }
        //        else
        //        {
        //            msg.Status = MessageStatus.Failed;
        //        }
        //        msg.ModifiedAt = DateTime.Now;
        //        msg.ModifiedBy = user.UserName;
        //        db.SaveChanges();
        //    }
        //}

        //public static void ProcessSms(long id)
        //{
        //    using (var db = new AppDbContext())
        //    {
        //        using (var transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var msg = db.Messages.First(x => x.Id == id);
        //                var user = db.Users.First(x => x.UserName == msg.CreatedBy);

        //                var numbers = msg.Receipient;
        //                numbers = numbers.Replace("-", "");
        //                numbers = numbers.Replace(" ", "");
        //                //strip the country code form it
        //                if (!numbers.StartsWith("233") && !numbers.StartsWith("0"))
        //                {
        //                    numbers = "233" + numbers;
        //                }
        //                if (numbers.StartsWith("00233"))
        //                {
        //                    numbers = "233" + numbers.Substring(5);
        //                }
        //                if (numbers.StartsWith("+233")) numbers = numbers.Replace("+233", "233");
        //                if (numbers.Length <= 10)
        //                {
        //                    if (numbers.StartsWith("02"))
        //                    {
        //                        numbers = "2332" + numbers.Substring(2);
        //                    }
        //                    else if (numbers.StartsWith("05"))
        //                    {
        //                        numbers = "2335" + numbers.Substring(2);
        //                    }
        //                }


        //                var infobipMessage = new InfobipHelpers.InfobipSmsMessage
        //                {
        //                    @from = SetupConfig.Setting.SmsMessageService.SenderId,
        //                    text = msg.Content,
        //                    to = numbers
        //                };
        //                //send the sms messages
        //                var client = new RestClient(SetupConfig.Setting.SmsMessageService.BaseUrl);
        //                var req = new RestRequest(Method.POST)
        //                {
        //                    RequestFormat = DataFormat.Json
        //                };
        //                req.AddHeader("Accept", "application/json");
        //                req.AddHeader("Content-Type", "application/json");
        //                req.AddHeader("Authorization", SetupConfig.Setting.SmsMessageService.BasicToken);

        //                var rq = JsonConvert.SerializeObject(infobipMessage);
        //                req.AddParameter("application/json", rq, ParameterType.RequestBody);

        //                var res = client.Execute(req);

        //                if (res.StatusCode == HttpStatusCode.OK)
        //                {
        //                    msg.Status = MessageStatus.Sent;

        //                }
        //                else
        //                {
        //                    msg.Status = MessageStatus.Failed;
        //                }
        //                msg.ModifiedAt = DateTime.Now;
        //                db.SaveChanges();
        //                transaction.Commit();
        //            }
        //            catch (Exception e)
        //            {
        //                transaction.Rollback();
        //                WebHelpers.ProcessException(e);
        //            }
        //        }
        //    }
        //}
        //public static void ProcessNotificationEmail(long id)
        //{
        //    try
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var msg = db.NotificationMessages.First(x => x.Id == id);

        //            var client = new RestClient
        //            {
        //                BaseUrl = new Uri("https://api.mailgun.net/v3"),
        //                Authenticator = new HttpBasicAuthenticator("api",
        //                    "key-2a3fec9de587928bde93299c20e1e376")
        //            };
        //            var request = new RestRequest();
        //            //request.
        //            request.AddParameter("domain",
        //                "blumamessage.6onelabs.com", ParameterType.UrlSegment);
        //            request.Resource = "{domain}/messages";
        //            request.AddParameter("from", SetupConfig.Setting.SmsMessageService.SenderId + " <mailgun@blumamessage.6onelabs.com>");
        //            request.AddParameter("to", msg.Receiver);
        //            request.AddParameter("subject", msg.Subject);
        //            request.AddParameter("html", msg.Message);
        //            request.AddParameter("text", "Bluma Messaging Service");

        //            request.Method = Method.POST;
        //            var res = client.Execute(request);

        //            msg.Status = res.StatusCode == HttpStatusCode.OK ? MessageStatus.Sent : MessageStatus.Failed;
        //            msg.ModifiedAt = DateTime.Now;
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        WebHelpers.ProcessException(e);
        //    }
        //}

        //public static void ProcessNotificationSms(long id)
        //{
        //    try
        //    {
        //        using (var db = new AppDbContext())
        //        {
        //            var msg = db.NotificationMessages.First(x => x.Id == id);

        //            var numbers = msg.Receiver;
        //            numbers = numbers.Replace("-", "");
        //            numbers = numbers.Replace(" ", "");
        //            //strip the country code form it
        //            if (numbers.StartsWith("00233"))
        //            {
        //                numbers = "233" + numbers.Substring(5);
        //            }
        //            if (numbers.StartsWith("+233")) numbers = numbers.Replace("+233", "233");
        //            if (numbers.Length <= 10)
        //            {
        //                if (numbers.StartsWith("02"))
        //                {
        //                    numbers = "2332" + numbers.Substring(2);
        //                }
        //                else if (numbers.StartsWith("05"))
        //                {
        //                    numbers = "2335" + numbers.Substring(2);
        //                }
        //            }


        //            var infobipMessage = new InfobipHelpers.InfobipSmsMessage
        //            {
        //                @from = SetupConfig.Setting.SmsMessageService.SenderId,
        //                text = msg.Message,
        //                to = numbers
        //            };
        //            //send the sms messages
        //            var client = new RestClient(SetupConfig.Setting.SmsMessageService.BaseUrl);
        //            var req = new RestRequest(Method.POST)
        //            {
        //                RequestFormat = DataFormat.Json
        //            };
        //            req.AddHeader("Accept", "application/json");
        //            req.AddHeader("Content-Type", "application/json");
        //            req.AddHeader("Authorization", SetupConfig.Setting.SmsMessageService.BasicToken);

        //            var rq = JsonConvert.SerializeObject(infobipMessage);
        //            req.AddParameter("application/json", rq, ParameterType.RequestBody);

        //            var res = client.Execute(req);

        //            if (res.StatusCode == HttpStatusCode.OK)
        //            {
        //                msg.Status = MessageStatus.Sent;
        //            }
        //            else
        //            {
        //                msg.Status = MessageStatus.Failed;
        //            }
        //            msg.ModifiedAt = DateTime.Now;
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        WebHelpers.ProcessException(e);
        //    }
        //}

        //public static string FixContactMessage(string message, Contact cont)
        //{
        //    var placeholders = SetupConfig.Setting.Wildcards.ToList();
        //    foreach (var w in placeholders)
        //    {
        //        switch (w)
        //        {
        //            case "[FULLNAME]":
        //                var name1 = string.IsNullOrEmpty(cont.Name) ? "" : cont.Name;
        //                message = message.Replace(w, name1);
        //                break;
        //            case "[NAME]":
        //                var name2 = string.IsNullOrEmpty(cont.Name) ? "" : cont.Name;
        //                message = message.Replace(w, name2);
        //                break;
        //            case "[EMAIL]":
        //                var email = string.IsNullOrEmpty(cont.Email) ? "EMAILADDRESS" : cont.Email;
        //                message = message.Replace(w, email);
        //                break;
        //            case "[PHONENUMBER]":
        //                var phone = string.IsNullOrEmpty(cont.PhoneNumber) ? "PHONENUMBER" : cont.PhoneNumber;
        //                message = message.Replace(w, phone);
        //                break;
        //            case "[DATEOFBIRTH]":
        //                var dob = cont.DateOfBirth.HasValue ? cont.DateOfBirth.Value.ToLongDateString() : "DOB";
        //                message = message.Replace(w, dob);
        //                break;
        //        }
        //    }
        //    return message;
        //}

        //public static string FixAnniversaryMessage(string message, Contact cont)
        //{
        //    var placeholders = SetupConfig.Setting.Wildcards.ToList();
        //    foreach (var w in placeholders)
        //    {
        //        switch (w)
        //        {
        //            case "[FULLNAME]":
        //                var name1 = string.IsNullOrEmpty(cont.Name) ? "" : cont.Name;
        //                message = message.Replace(w, name1);
        //                break;
        //            case "[NAME]":
        //                var name2 = string.IsNullOrEmpty(cont.Name) ? "" : cont.Name;
        //                message = message.Replace(w, name2);
        //                break;
        //            case "[EMAIL]":
        //                var email = string.IsNullOrEmpty(cont.Email) ? "EMAILADDRESS" : cont.Email;
        //                message = message.Replace(w, email);
        //                break;
        //            case "[PHONENUMBER]":
        //                var phone = string.IsNullOrEmpty(cont.PhoneNumber) ? "PHONENUMBER" : cont.PhoneNumber;
        //                message = message.Replace(w, phone);
        //                break;
        //            case "[DATEOFBIRTH]":
        //                var dob = cont.DateOfBirth.HasValue ? cont.DateOfBirth.Value.ToLongDateString() : "DOB";
        //                message = message.Replace(w, dob);
        //                break;
        //            case "[WORKAGE]":
        //                var workage = DateHelpers.CalculateAge(cont.EmploymentDate.Value);
        //                message = message.Replace(w, workage.ToString());
        //                break;
        //        }
        //    }
        //    return message;
        //}

        //public static string FixTempRecpMessage(string message, TemplatingRecipient cont)
        //{
        //    var placeholders = SetupConfig.Setting.Wildcards.ToList();
        //    foreach (var w in placeholders)
        //    {
        //        switch (w)
        //        {
        //            case "[FULLNAME]":
        //                message = message.Replace(w, cont.FullName);
        //                break;
        //            case "[EMAIL]":
        //                message = message.Replace(w, cont.Email);
        //                break;
        //            case "[PHONENUMBER]":
        //                message = message.Replace(w, cont.PhoneNumber);
        //                break;
        //            case "[CUSTOM1]":
        //                message = message.Replace(w, cont.Custom1);
        //                break;
        //            case "[CUSTOM2]":
        //                message = message.Replace(w, cont.Custom2);
        //                break;
        //            case "[CUSTOM3]":
        //                message = message.Replace(w, cont.Custom3);
        //                break;
        //            case "[CUSTOM4]":
        //                message = message.Replace(w, cont.Custom4);
        //                break;
        //            case "[CUSTOM5]":
        //                message = message.Replace(w, cont.Custom5);
        //                break;
        //        }
        //    }
        //    return message;
        //}
    }
}
