using System.Windows;
using System.Windows.Controls;

namespace JLS__
{
    /// <summary>
    /// Thank you: https://www.codeproject.com/Articles/1097390/Displaying-HTML-in-a-WPF-RichTextBox
    /// </summary>
    public class WebBrowserBehavior
    {
        public static readonly DependencyProperty BodyProperty =
          DependencyProperty.RegisterAttached("Body", typeof(string), typeof(WebBrowserBehavior),
           new PropertyMetadata(OnChanged));

        public static string GetBody(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(BodyProperty);
        }

        public static void SetBody(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(BodyProperty, body);
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
         ((WebBrowser)d).NavigateToString((string)e.NewValue);
    }
}
