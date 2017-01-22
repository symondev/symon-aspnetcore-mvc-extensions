using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using zxm.MailKit.Abstractions;
using Symon.AspNetCore.Mvc.Extensions.Exception;

namespace Symon.AspNetCore.Mvc.Extensions.Tests
{
    public class ExceptionHandlerMiddlewareTest
    {
        [Fact]
        public async Task TestExceptionLogger()
        {
            Mock<IMailSender> emailSenderMock = null;
            int processManualTimes = 0;

            var hostBuilder = new WebHostBuilder();
            hostBuilder.ConfigureServices(collection =>
            {
                emailSenderMock = new Mock<IMailSender>();
                emailSenderMock.Setup(p => p.SendEmail(It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<string>(), It.IsAny<string>())).Throws<NotImplementedException>();
                collection.AddSingleton<IMailSender>(provider => emailSenderMock.Object);
            });
            hostBuilder.Configure(app =>
            {
                //var options = new ExceptionHandlerOptions();
                //options.MailOptions = new MailOptions
                //{
                //    To = new List<MailAddress> { new MailAddress { Address = "123@test.com" }, new MailAddress { Address = "246@test.com" } },
                //    Subject = "Test Error"
                //};

                //options.ManualProcess = exception =>
                //{
                //    processManualTimes++;
                //};

                app.UseSymonExceptionHandler();
                app.Run(context =>
                {
                    throw new System.Exception("Server Error");
                });
            });

            using (var testServer = new TestServer(hostBuilder))
            {
                await Assert.ThrowsAsync<System.Exception>(async () => await testServer.CreateRequest("/").GetAsync());

                Assert.Equal(1, processManualTimes);

                emailSenderMock.Verify(
                        p => p.SendEmailAsync(It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            }
        }
    }
}
