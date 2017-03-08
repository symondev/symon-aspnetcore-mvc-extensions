using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using zxm.MailKit.Abstractions;

namespace Symon.AspNetCore.Mvc.Extensions.Exception
{
    public static class ExceptionHandlerAppBuilderExtensions
    {
        public static void UseSymonExceptionHandler(this IApplicationBuilder app, ExceptionMailHandlerOptions options = null)
        {
            if (options != null)
            {
                var serviceCollection = app.ApplicationServices.GetService<IServiceCollection>();
                serviceCollection.AddSingleton(options);

                Options.Create(options);
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(Process);
            });
        }

        private static async Task Process(HttpContext context)
        {
            
            var mailOptions = context.RequestServices.GetService<ExceptionMailHandlerOptions>();

            var mailSender = context.RequestServices.GetService<IMailSender>();

            var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger("ExceptionHandler");

            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();

            logger.LogError(0, exceptionHandler.Error, "An unhandled exception has occurred: " + exceptionHandler.Error.Message);

            var errMessage = BuildErrorMessage(exceptionHandler.Error, context);
            logger.LogError(errMessage);

            if (mailOptions != null && mailSender != null)
            {
                try
                {
                    await mailSender.SendEmailAsync(mailOptions.MailTos, mailOptions.MailSubject, errMessage);
                }
                catch (System.Exception ex2)
                {
                    logger.LogError(0, ex2, "An unhandled exception has occurred during send error email: " + ex2.Message);
                    logger.LogError(BuildErrorMessage(ex2));
                }
            }
        }

        private static string BuildErrorMessage(System.Exception ex, HttpContext context = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-------------------- Exception Details --------------------");
            GetErrorMessage(ex, sb);
            if (context != null)
            {
                sb.AppendLine("-------------------- Request Infomation --------------------");
                GetRequestInfo(context, sb);
            }
            return sb.ToString();
        }

        private static void GetRequestInfo(HttpContext context, StringBuilder sb)
        {
            sb.AppendLine($"Request Head: {JsonConvert.SerializeObject(context.Request.Headers)}");
            sb.AppendLine($"Request Host: {context.Request.Host}");
            sb.AppendLine($"Request Path: {context.Request.Path}");
            sb.AppendLine($"Request Query String: {context.Request.QueryString}");

            var bodyString = string.Empty;
            using (var stremReader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                bodyString = stremReader.ReadToEnd();
            }
            sb.AppendLine($"Request Body: {bodyString}");
        }

        private static void GetErrorMessage(System.Exception ex, StringBuilder sb)
        {
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"StackTrace:");
            sb.AppendLine(ex.StackTrace);

            if (ex.InnerException != null)
            {
                sb.AppendLine("-------------------- InnertException --------------------");
                GetErrorMessage(ex.InnerException, sb);
            }
        }
    }
}
