using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NetCoreStarter.Utils.Helpers
{
    public class WebHelpers
    {
        public static ApiResponse BuildResponse(HttpStatusCode statusCode, object data, string msg, long total)
        {
            var success = false;
            if (string.IsNullOrEmpty(msg)) msg = $"{total} record(s) found.";
            if (statusCode == HttpStatusCode.OK) success = true;
            return new ApiResponse(statusCode, data, msg, success, total);
        }

        /// <summary>
        /// Builds the erroe message
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <returns></returns>
        private static string ErrorMsg(Exception exception)
        {
            var validationException = exception as DbEntityValidationException;
            if (validationException != null)
            {
                var lines = validationException.EntityValidationErrors.Select(
                    x => new
                    {
                        name = x.Entry.Entity.GetType().Name.Split('_')[0],
                        errors = x.ValidationErrors.Select(y => y.PropertyName + ":" + y.ErrorMessage)
                    })
                                               .Select(x => $"{x.name} => {string.Join(",", x.errors)}");
                return string.Join("\r\n", lines);
            }

            var updateException = exception as DbUpdateException;
            if (updateException != null)
            {
                Exception innerException = updateException;
                while (innerException.InnerException != null) innerException = innerException.InnerException;
                if (innerException != updateException)
                {
                    if (innerException is SqlException)
                    {
                        var result = ProcessSqlExceptionMessage(innerException.Message);
                        if (!string.IsNullOrEmpty(result)) return result;
                    }
                }
                var entities = updateException.Entries.Select(x => x.Entity.GetType().Name.Split('_')[0])
                                              .Distinct()
                                              .Aggregate((a, b) => a + ", " + b);
                return ($"{innerException.Message} => {entities}");
            }

            var msg = exception.Message;
            if (exception.InnerException == null) return msg;
            msg = exception.InnerException.Message;

            if (exception.InnerException.InnerException == null) return msg;
            msg = exception.InnerException.InnerException.Message;

            if (exception.InnerException.InnerException.InnerException != null)
            {
                msg = exception.InnerException.InnerException.InnerException.Message;
            }

            return msg;
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="exception">The ex.</param>
        /// <returns></returns>
        public static ApiResponse ProcessException(Exception exception)
        {
            var msg = ErrorMsg(exception);
            return BuildResponse(HttpStatusCode.BadRequest, null, msg, 0);
        }

        /// <summary>
        /// Processes the SQL exception message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private static string ProcessSqlExceptionMessage(string message)
        {

            if (message.Contains("unique index"))
                return "Operation failed. Data is constrained to be unique.";
            return message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ?
                "This record is referenced by other records hence can not be deleted."
                : message;
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="values">The ASP.NET MVC model state values.</param>
        /// <returns></returns>
        public static ApiResponse ProcessException(ICollection<ModelState> values)
        {
            var msg = values.SelectMany(modelState => modelState.Errors)
                .Aggregate("", (current, error) => current + error.ErrorMessage + "\n");
            return BuildResponse(HttpStatusCode.BadRequest, null, msg, 0);
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <param name="identityResult">The identity result.</param>
        /// <returns></returns>
        public static ApiResponse ProcessException(IdentityResult identityResult)
        {
            var msg = identityResult.Errors.Aggregate("", (current, error) => current + error + "\n");
            return BuildResponse(HttpStatusCode.BadRequest, null, msg, 0);
        }
    }

}
