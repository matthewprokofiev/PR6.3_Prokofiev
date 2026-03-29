using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PR14
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Регистрирует нового пользователя.
        /// Возвращает true, если регистрация успешна; иначе false.
        /// </summary>
        public bool Register(string login, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(email))
                return false;

            if (password != confirmPassword)
                return false;

            if (password.Length < 4)
                return false;

            var emailRegex = new System.Text.RegularExpressions.Regex(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                return false;

            var db = Manager.GetContext();

            if (db.Users.Any(u => u.Login == login))
                return false;

            if (db.Users.Any(u => u.Email == email))
                return false;

            var newUser = new Users
            {
                Login = login,
                Password = password,
                Email = email
            };

            db.Users.Add(newUser);
            db.SaveChanges();
            return true;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool success = Register(
                TxtLogin.Text,
                TxtEmail.Text,
                TxtPassword.Password,
                TxtPasswordConfirm.Password);

            if (success)
            {
                MessageBox.Show("Регистрация прошла успешно!");
                Manager.MainFrame.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Ошибка регистрации. Проверьте корректность введённых данных.");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) =>
            Manager.MainFrame.GoBack();
    }
}
