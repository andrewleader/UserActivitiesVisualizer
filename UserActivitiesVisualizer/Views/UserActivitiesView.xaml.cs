using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UserActivitiesVisualizer.Helpers;
using UserActivitiesVisualizer.ViewModels.UserActivities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UserActivitiesVisualizer.Views
{
    public sealed partial class UserActivitiesView : UserControl
    {
        public UserActivitiesViewModel ViewModel => DataContext as UserActivitiesViewModel;

        public UserActivitiesView()
        {
            this.InitializeComponent();
            this.DataContextChanged += UserActivitesView_DataContextChanged;
        }

        private UserActivitiesViewModel _oldViewModel;
        private PropertyChangedEventHandler _viewModelPropertyChangedHandler;
        private void UserActivitesView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_viewModelPropertyChangedHandler == null)
            {
                _viewModelPropertyChangedHandler = new WeakEventHandler<PropertyChangedEventArgs>(ViewModel_PropertyChanged).Handler;
            }

            if (_oldViewModel != null)
            {
                _oldViewModel.PropertyChanged -= _viewModelPropertyChangedHandler;
            }

            if (ViewModel != null)
            {
                // Have to set these programmatically, otherwise SelectedItem sets itself to null which
                // clears out the CurrentDocument which results in nothing being displayed the first time
                // the page loads
                ListViewTabs.ItemsSource = ViewModel.OpenActivities;
                ViewModel.PropertyChanged += _viewModelPropertyChangedHandler;
                UpdateSelectedItem();
            }

            _oldViewModel = ViewModel;
        }

        private void UpdateSelectedItem()
        {
            ListViewTabs.SelectedItem = ViewModel?.CurrentActivity;
            if (ListViewTabs.SelectedItem != null)
            {
                ListViewTabs.ScrollIntoView(ListViewTabs.SelectedItem);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.CurrentActivity):
                    UpdateSelectedItem();
                    break;
            }
        }

        private void ListViewTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null && ListViewTabs.SelectedItem != null)
            {
                ViewModel.CurrentActivity = ListViewTabs.SelectedItem as UserActivityViewModel;
            }
        }

        private void CommandBar_Opening(object sender, object e)
        {
            foreach (var b in AppBarButtons())
            {
                b.IsCompact = false;
            }
        }

        private void CommandBar_Closing(object sender, object e)
        {
            foreach (var b in AppBarButtons())
            {
                b.IsCompact = true;
            }
        }

        private IEnumerable<AppBarButton> AppBarButtons()
        {
            foreach (var b in StackPanelMainAppBarButtons.Children.OfType<AppBarButton>())
            {
                yield return b;
            }
        }

        private async void AppBarSave_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("SaveClicked");

            try
            {
                if (ViewModel.CurrentActivity == null)
                {
                    return;
                }

                AppBarSave.IsEnabled = false;
                await ViewModel.CurrentActivity.SaveAsync();
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString(), "Failed to save").ShowAsync();
            }
            finally
            {
                AppBarSave.IsEnabled = true;
            }
        }

        private async void AppBarSaveAs_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("SaveAsClicked");

            try
            {
                if (ViewModel.CurrentActivity == null)
                {
                    return;
                }

                AppBarSaveAs.IsEnabled = false;
                await ViewModel.CurrentActivity.SaveAsAsync();
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString(), "Failed to save").ShowAsync();
            }
            finally
            {
                AppBarSaveAs.IsEnabled = true;
            }
        }

        private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 400)
            {
                VisualStateManager.GoToState(this, "Compact", true);
                EnsureContentIsInPivot();
            }
            else if (e.NewSize.Width < 800)
            {
                VisualStateManager.GoToState(this, "SemiCompact", true);
                EnsureContentIsInPivot();
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
                EnsureContentIsInSplitScreen();
            }
        }

        private void EnsureContentIsInSplitScreen()
        {
            if (EditorColumn.Child == null)
            {
                MoveContent(EditorPivot, EditorColumn);
                //MoveContent(PreviewPivot, PreviewColumn);
            }
        }

        private void EnsureContentIsInPivot()
        {
            if (EditorPivot.Child == null)
            {
                MoveContent(EditorColumn, EditorPivot);
                //MoveContent(PreviewColumn, PreviewPivot);
            }
        }

        private void MoveContent(Border source, Border destination)
        {
            UIElement copy = source.Child;
            source.Child = null;
            destination.Child = copy;
        }

        private void ListViewAddDocumentItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            FlyoutAddDocument.Hide();
            var dontWait = ViewModel.AddActivityAsync(e.ClickedItem as AddUserActivityListItem);
        }
    }
}
