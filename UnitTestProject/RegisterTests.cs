using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using PR14;

namespace UnitTestProject
{
    [TestClass]
    public class RegisterTests
    {
        // Позитивные тесты регистрации
        [TestMethod]
        [DataRow("newuser_test1", "pass1234", "pass1234")]
        [DataRow("newuser_test2", "abcd", "abcd")]
        [DataRow("newuser_test3", "qwerty123", "qwerty123")]
        public void RegisterTestSuccess(string login, string password, string confirm)
        {
            var page = new RegisterPage();

            var db = Manager.GetContext();
            var existing = db.Users.FirstOrDefault(u => u.Login == login);
            if (existing != null)
            {
                db.Users.Remove(existing);
                db.SaveChanges();
            }

            bool result = page.Register(login, password, confirm);
            Assert.IsTrue(result,
                $"Позитивный тест провален: регистрация '{login}' не прошла.");
        }

        // Негативные тесты регистрации
        [TestMethod]
        [DataRow("", "", "")]
        [DataRow("", "password", "password")]
        [DataRow("somelogin", "", "")]
        [DataRow("newuser_1", "password1", "password2")]
        [DataRow("newuser_2", "ab", "ab")]
        [DataRow("   ", "   ", "   ")]
        [DataRow("user1", "newpassword", "newpassword")]
        public void RegisterTestFail(string login, string password, string confirm)
        {
            var page = new RegisterPage();
            bool result = page.Register(login, password, confirm);
            Assert.IsFalse(result,
                $"Негативный тест провален: система зарегистрировала '{login}' когда не должна была.");
        }
    }
}
