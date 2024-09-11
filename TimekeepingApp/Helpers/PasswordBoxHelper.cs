using System.Windows;
using System.Windows.Controls;

namespace TimekeepingApp.Helpers
{
    public static class PasswordBoxHelper
    {
        // Attached property for binding the password
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        // Attached property for enabling binding on PasswordBox
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        // Private attached property to prevent recursive updates
        private static readonly DependencyProperty UpdatingPasswordProperty =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPasswordProperty, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPasswordProperty);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                // Remove the event handler to prevent recursive updates
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                if (!(bool)passwordBox.GetValue(UpdatingPasswordProperty))
                {
                    passwordBox.Password = (string)e.NewValue;
                }

                // Add the event handler back to handle future changes
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                var wasBound = (bool)e.OldValue;
                var needToBind = (bool)e.NewValue;

                if (wasBound)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }

                if (needToBind)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Set the UpdatingPassword flag to prevent recursive updates
                passwordBox.SetValue(UpdatingPasswordProperty, true);

                // Update the bound property with the current password
                SetBoundPassword(passwordBox, passwordBox.Password);

                // Reset the UpdatingPassword flag
                passwordBox.SetValue(UpdatingPasswordProperty, false);
            }
        }
    }
}
