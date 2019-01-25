using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UserActivitiesVisualizer.Controls
{
    public sealed partial class TextBoxUriControl : UserControl
    {
        public TextBoxUriControl()
        {
            this.InitializeComponent();
        }

        public string Header
        {
            get => TextBox.Header?.ToString();
            set => TextBox.Header = value;
        }

        public Uri Uri
        {
            get { object val = GetValue(UriProperty); if (val is Uri uri) return uri; return new Uri("empty:cool"); }
            set { SetValue(UriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Uri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(Uri), typeof(TextBoxUriControl), new PropertyMetadata(null, OnUriChanged));

        private static void OnUriChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TextBoxUriControl).OnUriChanged(e);
        }

        private void OnUriChanged(DependencyPropertyChangedEventArgs e)
        {
            TextBox.Text = Uri?.ToString() ?? "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Uri.TryCreate(TextBox.Text, UriKind.RelativeOrAbsolute, out Uri result))
            {
                try
                {
                    if (!result.Equals(Uri))
                    {
                        Uri = result;
                    }
                    Border.BorderThickness = new Thickness(0);
                    return;
                }
                catch { }
            }

            Border.BorderThickness = new Thickness(2);
        }
    }
}
