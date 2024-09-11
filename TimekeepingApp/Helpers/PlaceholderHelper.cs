using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TimekeepingApp.Helpers
{
    public static class PlaceholderHelper
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.RegisterAttached(
                "PlaceholderText",
                typeof(string),
                typeof(PlaceholderHelper),
                new PropertyMetadata(string.Empty, OnPlaceholderTextChanged));

        public static string GetPlaceholderText(Control control)
        {
            return (string)control.GetValue(PlaceholderTextProperty);
        }

        public static void SetPlaceholderText(Control control, string value)
        {
            control.SetValue(PlaceholderTextProperty, value);
        }

        private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Loaded += (s, ev) => UpdatePlaceholder(textBox);
                textBox.GotFocus += (s, ev) => RemovePlaceholder(textBox);
                textBox.LostFocus += (s, ev) => UpdatePlaceholder(textBox);
            }
            else if (d is PasswordBox passwordBox)
            {
                passwordBox.Loaded += (s, ev) => UpdatePlaceholder(passwordBox);
                passwordBox.GotFocus += (s, ev) => RemovePlaceholder(passwordBox);
                passwordBox.LostFocus += (s, ev) => UpdatePlaceholder(passwordBox);
            }
        }

        private static void UpdatePlaceholder(Control control)
        {
            if (control is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = GetPlaceholderText(textBox);
                textBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else if (control is PasswordBox passwordBox && string.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.Tag = passwordBox.PasswordChar.ToString();
                passwordBox.Password = GetPlaceholderText(passwordBox);
                passwordBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private static void RemovePlaceholder(Control control)
        {
            if (control is TextBox textBox && textBox.Text == GetPlaceholderText(textBox))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (control is PasswordBox passwordBox && passwordBox.Password == GetPlaceholderText(passwordBox))
            {
                passwordBox.Password = string.Empty;
                passwordBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
