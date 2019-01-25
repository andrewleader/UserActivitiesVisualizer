using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UserActivitiesVisualizer.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UserActivitiesVisualizer.Views
{
    public class EditableObjectView : StackPanel
    {


        public object EditableObject
        {
            get { return (object)GetValue(EditableObjectProperty); }
            set { SetValue(EditableObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditableObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditableObjectProperty =
            DependencyProperty.Register("EditableObject", typeof(object), typeof(EditableObjectView), new PropertyMetadata(null, OnEditableObjectChanged));

        private static void OnEditableObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EditableObjectView).OnEditableObjectChanged(e);
        }

        private void OnEditableObjectChanged(DependencyPropertyChangedEventArgs e)
        {
            Children.Clear();

            if (EditableObject == null)
            {
                return;
            }

            foreach (var prop in EditableObject.GetType().GetTypeInfo().DeclaredProperties)
            {
                var propType = prop.PropertyType;

                if (propType == typeof(string))
                {
                    AddStringProperty(prop);
                }
                else if (propType == typeof(Uri))
                {
                    AddUriProperty(prop);
                }
                else if (propType == typeof(JObject))
                {
                    AddJObjectProperty(prop);
                }
                else if (propType.IsClass)
                {
                    AddSubEditableObject(prop);
                }
            }
        }

        private void AddStringProperty(PropertyInfo prop)
        {
            TextBox tb = new TextBox()
            {
                Header = prop.Name
            };
            tb.SetBinding(TextBox.TextProperty, new Binding()
            {
                Source = EditableObject,
                Path = new PropertyPath(prop.Name),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            AddControl(tb);
        }

        private void AddUriProperty(PropertyInfo prop)
        {
            TextBoxUriControl tb = new TextBoxUriControl()
            {
                Header = prop.Name
            };
            tb.SetBinding(TextBoxUriControl.UriProperty, new Binding()
            {
                Source = EditableObject,
                Path = new PropertyPath(prop.Name),
                Mode = BindingMode.TwoWay
            });
            AddControl(tb);
        }

        private void AddJObjectProperty(PropertyInfo p)
        {
            JObjectControl tb = new JObjectControl()
            {
                Header = p.Name
            };
            tb.SetBinding(JObjectControl.JObjectProperty, new Binding()
            {
                Source = EditableObject,
                Path = new PropertyPath(p.Name),
                Mode = BindingMode.TwoWay
            });
            AddControl(tb);
        }

        private void AddSubEditableObject(PropertyInfo p)
        {
            StackPanel sp = new StackPanel();
            sp.Children.Add(new TextBlock()
            {
                Text = p.Name
            });

            var subEditable = new EditableObjectView();
            subEditable.SetBinding(EditableObjectView.EditableObjectProperty, new Binding()
            {
                Source = EditableObject,
                Path = new PropertyPath(p.Name)
            });

            sp.Children.Add(new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(6),
                Child = subEditable
            });

            AddControl(sp);
        }

        private void AddControl(FrameworkElement element)
        {
            if (Children.Count > 0)
            {
                Children.Add(new Border() { Height = 6 });
            }
            Children.Add(element);
        }
    }
}
