using AdaptiveCards.Rendering.Uwp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UserActivitiesVisualizer.Views.Previews
{
    public sealed partial class TimelineCardRenderer : UserControl
    {
        private AdaptiveCardRenderer _renderer;

        public TimelineCardRenderer()
        {
            this.InitializeComponent();
        }



        public bool RenderedSuccessfully
        {
            get { return (bool)GetValue(RenderedSuccessfullyProperty); }
            set { SetValue(RenderedSuccessfullyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderedSuccessfully.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderedSuccessfullyProperty =
            DependencyProperty.Register("RenderedSuccessfully", typeof(bool), typeof(TimelineCardRenderer), new PropertyMetadata(false));



        public JObject CardPayload
        {
            get { return (JObject)GetValue(CardPayloadProperty); }
            set { SetValue(CardPayloadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CardPayload.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CardPayloadProperty =
            DependencyProperty.Register("CardPayload", typeof(JObject), typeof(TimelineCardRenderer), new PropertyMetadata(null, OnCardPayloadChanged));

        private static void OnCardPayloadChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TimelineCardRenderer).OnCardPayloadChanged(e);
        }

        private void OnCardPayloadChanged(DependencyPropertyChangedEventArgs e)
        {
            TriggerUpdate();
        }

        private bool _isUpdating;
        private async void TriggerUpdate()
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;

            try
            {
                if (_renderer == null)
                {
                    _renderer = new AdaptiveCardRenderer();
                    _renderer.SetFixedDimensions(320, 176);

                    var hostConfigsFolder = await Package.Current.InstalledLocation.GetFolderAsync("HostConfigs");
                    var hostConfigFile = await hostConfigsFolder.GetFileAsync("TimelineWindows.json");
                    _renderer.HostConfig = AdaptiveHostConfig.FromJsonString(await FileIO.ReadTextAsync(hostConfigFile)).HostConfig;
                }

                if (CardPayload == null)
                {
                    RenderedElement = null;
                    RenderedSuccessfully = false;
                }
                else
                {
                    var renderResult = _renderer.RenderAdaptiveCardFromJsonString(CardPayload.ToString());
                    RenderedElement = renderResult.FrameworkElement;
                    RenderedSuccessfully = RenderedElement != null;
                }
            }
            catch
            {
                RenderedElement = null;
                RenderedSuccessfully = false;
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public FrameworkElement RenderedElement
        {
            get { return (FrameworkElement)GetValue(RenderedElementProperty); }
            private set { SetValue(RenderedElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderedElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderedElementProperty =
            DependencyProperty.Register("RenderedElement", typeof(FrameworkElement), typeof(TimelineCardRenderer), new PropertyMetadata(null));


    }
}
