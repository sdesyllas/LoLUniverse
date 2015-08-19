using LoLUniverse.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoLUniverse.Tests.Utilities
{
    [TestClass]
    public class MailSenderTests
    {
        [TestMethod]
        public void SimpleEmailTest()
        {
            // Arrange

            // Act
            var response = EmailSender.SendEmail("feniastavra@hotmail.com", "Test", "this is a test email from LoLUniverse app!");

            // Assert
            Assert.IsTrue(response);
        }
    }
}
