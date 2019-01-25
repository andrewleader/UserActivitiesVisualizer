using Newtonsoft.Json.Linq;
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
    public sealed partial class JObjectControl : UserControl
    {
        public JObjectControl()
        {
            this.InitializeComponent();
        }

        public string Header
        {
            get => TextBox.Header?.ToString();
            set => TextBox.Header = value;
        }

        public JObject JObject
        {
            get { return GetValue(JObjectProperty) as JObject; }
            set { SetValue(JObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty JObjectProperty =
            DependencyProperty.Register("JObject", typeof(JObject), typeof(JObjectControl), new PropertyMetadata(null, OnJObjectChanged));

        private static void OnJObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as JObjectControl).OnJObjectChanged(e);
        }

        private void OnJObjectChanged(DependencyPropertyChangedEventArgs e)
        {
            TextBox.Text = JObject?.ToString() ?? "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextBox.Text))
                {
                    if (JObject != null)
                    {
                        JObject = null;
                    }
                    Border.BorderThickness = new Thickness(0);
                    return;
                }
                var newJObject = JObject.Parse(TextBox.Text);
                if (JObject != null && JObject.ToString() == newJObject.ToString())
                {
                    Border.BorderThickness = new Thickness(0);
                    return;
                }
                JObject = newJObject;
                Border.BorderThickness = new Thickness(0);
                return;
            }
            catch { }

            Border.BorderThickness = new Thickness(2);
        }
    }
}
