using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using PR14;

namespace UnitTestProject
{
    /// <summary>
    /// Автоматизированные тесты капчи.
    /// Тестируются три метода: IsCaptchaRequired, GenerateCaptchaText, ValidateCaptcha.
    /// </summary>
    [TestClass]
    public class CaptchaTests
    {
        // Проверка необходимости капчи
        [TestMethod]
        public void IsCaptchaRequired_ZeroAttempts_ReturnsFalse()
            => Assert.IsFalse(LoginPage.IsCaptchaRequired(0));

        [TestMethod]
        public void IsCaptchaRequired_OneAttempt_ReturnsFalse()
            => Assert.IsFalse(LoginPage.IsCaptchaRequired(1));

        [TestMethod]
        public void IsCaptchaRequired_TwoAttempts_ReturnsFalse()
            => Assert.IsFalse(LoginPage.IsCaptchaRequired(2));

        [TestMethod]
        public void IsCaptchaRequired_ThreeAttempts_ReturnsTrue()
            => Assert.IsTrue(LoginPage.IsCaptchaRequired(3));

        [TestMethod]
        public void IsCaptchaRequired_FourAttempts_ReturnsTrue()
            => Assert.IsTrue(LoginPage.IsCaptchaRequired(4));

        [TestMethod]
        public void IsCaptchaRequired_NegativeAttempts_ReturnsFalse()
            => Assert.IsFalse(LoginPage.IsCaptchaRequired(-1));

        [TestMethod]
        public void IsCaptchaRequired_ThresholdIsExactlyThree()
        {
            Assert.IsFalse(LoginPage.IsCaptchaRequired(2), "До порога капчи нет");
            Assert.IsTrue(LoginPage.IsCaptchaRequired(3),  "На пороге капча появляется");
        }

        // Проверка генерации капчи
        [TestMethod]
        public void GenerateCaptchaText_DefaultLength_Returns5Chars()
            => Assert.AreEqual(5, LoginPage.GenerateCaptchaText().Length);

        [TestMethod]
        public void GenerateCaptchaText_Length1_Returns1Char()
            => Assert.AreEqual(1, LoginPage.GenerateCaptchaText(1).Length);

        [TestMethod]
        public void GenerateCaptchaText_Length10_Returns10Chars()
            => Assert.AreEqual(10, LoginPage.GenerateCaptchaText(10).Length);

        [TestMethod]
        public void GenerateCaptchaText_ZeroLength_ReturnsEmpty()
            => Assert.AreEqual(string.Empty, LoginPage.GenerateCaptchaText(0));

        [TestMethod]
        public void GenerateCaptchaText_NegativeLength_ReturnsEmpty()
            => Assert.AreEqual(string.Empty, LoginPage.GenerateCaptchaText(-5));

        [TestMethod]
        public void GenerateCaptchaText_IsNotNull()
            => Assert.IsNotNull(LoginPage.GenerateCaptchaText());

        [TestMethod]
        public void GenerateCaptchaText_IsNotEmpty()
            => Assert.IsFalse(string.IsNullOrEmpty(LoginPage.GenerateCaptchaText()));

        [TestMethod]
        public void GenerateCaptchaText_ContainsOnlyUppercaseOrDigits()
        {
            string captcha = LoginPage.GenerateCaptchaText(20);
            bool allValid = captcha.All(c => char.IsUpper(c) || char.IsDigit(c));
            Assert.IsTrue(allValid, $"Недопустимые символы в капче: {captcha}");
        }

        [TestMethod]
        public void GenerateCaptchaText_TwoCalls_UsuallyReturnDifferentValues()
        {
            bool foundDifference = false;
            for (int i = 0; i < 10; i++)
            {
                string first = LoginPage.GenerateCaptchaText();
                System.Threading.Thread.Sleep(1);
                string second = LoginPage.GenerateCaptchaText();

                if (first != second)
                {
                    foundDifference = true;
                    break;
                }
            }
            Assert.IsTrue(foundDifference, "Генератор выдаёт одинаковые строки — нет случайности");
        }

        [TestMethod]
        public void GenerateCaptchaText_LargeLength_DoesNotThrow()
        {
            string result = LoginPage.GenerateCaptchaText(100);
            Assert.AreEqual(100, result.Length);
        }

        //  Проверка ввода капчи
        [TestMethod]
        public void ValidateCaptcha_CorrectInput_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("ABCDE", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_WrongInput_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("WRONG", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_EmptyInput_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_NullInput_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha(null, "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_NullExpected_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("ABCDE", null));

        [TestMethod]
        public void ValidateCaptcha_BothNull_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha(null, null));

        [TestMethod]
        public void ValidateCaptcha_BothEmpty_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("", ""));

        [TestMethod]
        public void ValidateCaptcha_LowercaseInput_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("abcde", "ABCDE"),
                "Регистр должен игнорироваться");

        [TestMethod]
        public void ValidateCaptcha_MixedCaseInput_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("AbCdE", "ABCDE"),
                "Смешанный регистр должен работать");

        [TestMethod]
        public void ValidateCaptcha_InputWithLeadingSpaces_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("  ABCDE", "ABCDE"),
                "Пробелы в начале должны обрезаться");

        [TestMethod]
        public void ValidateCaptcha_InputWithTrailingSpaces_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("ABCDE  ", "ABCDE"),
                "Пробелы в конце должны обрезаться");

        [TestMethod]
        public void ValidateCaptcha_InputWithBothSideSpaces_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("  ABCDE  ", "ABCDE"),
                "Пробелы с обеих сторон должны обрезаться");

        [TestMethod]
        public void ValidateCaptcha_InputLongerThanExpected_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("ABCDEF", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_InputShorterThanExpected_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("ABCD", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_OnlySpacesInput_ReturnsFalse()
            => Assert.IsFalse(LoginPage.ValidateCaptcha("     ", "ABCDE"));

        [TestMethod]
        public void ValidateCaptcha_NumericCaptcha_ReturnsTrue()
            => Assert.IsTrue(LoginPage.ValidateCaptcha("23456", "23456"));

        [TestMethod]
        public void ValidateCaptcha_WithRealGeneratedCaptcha_Roundtrip()
        {
            string generated = LoginPage.GenerateCaptchaText();
            Assert.IsTrue(LoginPage.ValidateCaptcha(generated, generated),
                "Сгенерированная капча должна проходить проверку сама с собой");
        }

        [TestMethod]
        public void ValidateCaptcha_LowercaseRealCaptcha_Roundtrip()
        {
            string generated = LoginPage.GenerateCaptchaText();
            Assert.IsTrue(LoginPage.ValidateCaptcha(generated.ToLower(), generated),
                "Строчный вариант сгенерированной капчи должен проходить проверку");
        }
    }
}
