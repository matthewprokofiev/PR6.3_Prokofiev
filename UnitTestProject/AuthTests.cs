using Microsoft.VisualStudio.TestTools.UnitTesting;
using PR14;

namespace UnitTestProject
{
    [TestClass]
    public class AuthTests
    {
        // Позитивные тесты
        [TestMethod]
        [DataRow("admin", "admin")]
        [DataRow("user1", "12345")]
        [DataRow("user2", "12345")]
        public void AuthTestSuccess(string login, string password)
        {
            var page = new LoginPage();
            bool result = page.Auth(login, password);
            Assert.IsTrue(result,
                $"Позитивный тест провален: пользователь '{login}' не смог войти.");
        }

        // Негативные тесты
        [TestMethod]
        [DataRow("", "")]
        [DataRow("", "password1")]
        [DataRow("user1", "")]
        [DataRow("   ", "   ")]
        [DataRow("nonexistent_user_xyz", "anypassword")]
        [DataRow("user1", "wrongpassword")]
        [DataRow("wronglogin", "password1")]
        [DataRow("' OR '1'='1", "' OR '1'='1")]
        [DataRow("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "bbbbbbbbb")]
        public void AuthTestFail(string login, string password)
        {
            var page = new LoginPage();
            bool result = page.Auth(login, password);
            Assert.IsFalse(result,
                $"Негативный тест провален: система пропустила вход с логином='{login}', паролем='{password}'.");
        }
    }
}
