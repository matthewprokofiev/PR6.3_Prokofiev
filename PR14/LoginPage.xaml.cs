using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PR14
{
    public partial class LoginPage : Page
    {
        private int _failedAttempts = 0;
        private string _currentCaptcha = string.Empty;

        public LoginPage()
        {
            InitializeComponent();
            CaptchaPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>Проверяет логин и пароль по БД.</summary>
        public bool Auth(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            var db = Manager.GetContext();
            var user = db.Users.FirstOrDefault(
                u => u.Login == login && u.Password == password);

            if (user != null)
            {
                Manager.CurrentUser = user;
                return true;
            }
            return false;
        }

        /// <summary>Нужна ли капча? Да — если 3 и более неверных попыток.</summary>
        public static bool IsCaptchaRequired(int failedAttempts)
            => failedAttempts >= 3;

        /// <summary>
        /// Генерирует случайный текст капчи.
        /// Исключены визуально похожие символы: 0/O, 1/I/l.
        /// </summary>
        public static string GenerateCaptchaText(int length = 5)
        {
            if (length <= 0) return string.Empty;

            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var rng = new Random();
            return new string(
                Enumerable.Range(0, length)
                          .Select(_ => chars[rng.Next(chars.Length)])
                          .ToArray());
        }

        /// <summary>
        /// Проверяет введённую капчу.
        /// Регистр игнорируется, пробелы по краям обрезаются.
        /// </summary>
        public static bool ValidateCaptcha(string input, string expected)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(expected))
                return false;

            return string.Equals(
                input.Trim(), expected.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Генерирует изображение капчи с шумом.</summary>
        public static BitmapSource GenerateCaptchaImage(string text)
        {
            const int width = 160, height = 50;
            var rng = new Random();

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                for (int i = 0; i < 8; i++)
                {
                    var pen = new Pen(new SolidColorBrush(Color.FromRgb(
                        (byte)rng.Next(150, 220),
                        (byte)rng.Next(150, 220),
                        (byte)rng.Next(150, 220))), 1.5);
                    dc.DrawLine(pen,
                        new Point(rng.Next(width), rng.Next(height)),
                        new Point(rng.Next(width), rng.Next(height)));
                }

                for (int i = 0; i < 80; i++)
                {
                    dc.DrawEllipse(
                        new SolidColorBrush(Color.FromRgb(
                            (byte)rng.Next(100, 200),
                            (byte)rng.Next(100, 200),
                            (byte)rng.Next(100, 200))),
                        null,
                        new Point(rng.Next(width), rng.Next(height)),
                        1.5, 1.5);
                }

                double x = 10;
                foreach (char c in text)
                {
                    double offsetY = rng.Next(-5, 5);
                    var ft = new FormattedText(
                        c.ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        26,
                        new SolidColorBrush(Color.FromRgb(
                            (byte)rng.Next(0, 80),
                            (byte)rng.Next(0, 80),
                            (byte)rng.Next(0, 80))),
                        96);
                    dc.DrawText(ft, new Point(x, 8 + offsetY));
                    x += ft.Width + 2;
                }
            }

            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            return rtb;
        }

        private void RefreshCaptcha()
        {
            _currentCaptcha = GenerateCaptchaText();
            CaptchaImage.Source = GenerateCaptchaImage(_currentCaptcha);
            TxtCaptcha.Clear();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (IsCaptchaRequired(_failedAttempts))
            {
                if (!ValidateCaptcha(TxtCaptcha.Text, _currentCaptcha))
                {
                    MessageBox.Show("Неверная капча. Попробуйте ещё раз.");
                    RefreshCaptcha();
                    return;
                }
            }

            bool success = Auth(TxtLogin.Text, TxtPassword.Password);

            if (success)
            {
                _failedAttempts = 0;
                CaptchaPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Успешный вход!");
                Manager.MainFrame.Navigate(new MainPage());
            }
            else
            {
                _failedAttempts++;
                MessageBox.Show("Неверный логин или пароль!");

                if (IsCaptchaRequired(_failedAttempts))
                {
                    CaptchaPanel.Visibility = Visibility.Visible;
                    RefreshCaptcha();
                }
            }
        }

        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
            => RefreshCaptcha();

        private void BtnReg_Click(object sender, RoutedEventArgs e)
            => Manager.MainFrame.Navigate(new RegisterPage());

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => Manager.MainFrame.GoBack();
    }
}
