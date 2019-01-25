using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivitiesVisualizer.Settings;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivitiesViewModel : BindableBase
    {
        public UserActivitiesViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _currentActivityToken = new SettingsValueHelper<string>(this.GetType().Name + "CurrDocToken", null);
                Settings = GetSettings();

                await LoadFilesAsync();

                IsEnabled = true;
            }
            catch (Exception ex)
            {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public ObservableCollection<UserActivityViewModel> OpenActivities { get; private set; } = new ObservableCollection<UserActivityViewModel>();

        private SettingsValueHelper<string> _currentActivityToken;

        private UserActivityViewModel _currentActivity;
        public UserActivityViewModel CurrentActivity
        {
            get { return _currentActivity; }
            set
            {
                if (value != _currentActivity)
                {
                    if (_currentActivity != null)
                    {
                        _currentActivity.TriggerEndUserActivity();
                    }
                }

                SetProperty(ref _currentActivity, value);

                if (value != null)
                {
                    value.TriggerRecordUserActivity();
                    _currentActivityToken.Value = value.Token;
                    //value.ReloadIfNeeded();
                }

                else
                {
                    _currentActivityToken.Value = null;
                }
            }
        }

        public ObservableCollection<AddUserActivityListItem> AddActivityItems { get; private set; } = new ObservableCollection<AddUserActivityListItem>()
        {
            AddUserActivityListItem.NewActivity,
            AddUserActivityListItem.OpenActivity
        };

        public PayloadEditorSettings Settings { get; private set; }

        protected PayloadEditorSettings GetSettings()
        {
            return UserActivitiesVisualizer.Settings.Settings.UserActivityPayloadEditorSettings;
        }

        private void NewActivity()
        {
            AddActivity(CreateNewActivity());
            CurrentActivity = OpenActivities.Last();
        }

        protected UserActivityViewModel CreateNewActivity()
        {
            return UserActivityViewModel.CreateNew();
        }

        private async void OpenActivity()
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.FileTypeFilter.Add(".json");

                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    string token = StorageApplicationPermissions.FutureAccessList.Add(file);
                    AddAndSelectActivity(await LoadFromFileAsync(file, token, isSaveable: true));
                }
                else
                {
                }
            }
            catch { }
        }

        public async Task AddActivityAsync(AddUserActivityListItem doc)
        {
            if (doc == AddUserActivityListItem.NewActivity)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("NewDocumentClicked");
                NewActivity();
                return;
            }

            if (doc == AddUserActivityListItem.OpenActivity)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("OpenDocumentClicked");
                OpenActivity();
                return;
            }

            //Microsoft.HockeyApp.HockeyClient.Current.TrackEvent("OpenSampleClicked", new Dictionary<string, string>()
            //{
            //    { "SampleFile", doc.File.Name }
            //});

            AddAndSelectActivity(await LoadFromFileAsync(doc.File, "SampleFile:" + doc.File.Name, isSaveable: false));
        }

        private async void AddAndSelectActivity(UserActivityViewModel doc)
        {
            AddActivity(doc);
            CurrentActivity = OpenActivities.LastOrDefault();
            await SaveFileTokensAsync();
        }

        private void AddActivity(UserActivityViewModel doc)
        {
            OpenActivities.Add(doc);
            doc.OnRequestClose += Doc_OnRequestClose;
            doc.OnRequestSaveFileTokens += Doc_OnRequestSaveFileTokens;
        }

        private async void Doc_OnRequestSaveFileTokens(object sender, EventArgs e)
        {
            try
            {
                await SaveFileTokensAsync();
                _currentActivityToken.Value = _currentActivity?.Token;
            }
            catch { }
        }

        private void Doc_OnRequestClose(object sender, EventArgs e)
        {
            CloseActivity(sender as UserActivityViewModel);
        }

        protected async Task<UserActivityViewModel> LoadFromFileAsync(StorageFile file, string token, bool isSaveable)
        {
            return await UserActivityViewModel.CreateFromFileAsync(file, token, isSaveable);
        }

        private async Task LoadFilesAsync()
        {
            OpenActivities.Clear();

            var tokens = await Settings.GetFileTokensAsync();
            var documents = new List<UserActivityViewModel>();
            foreach (string token in tokens)
            {
                try
                {
                    StorageFile file;
                    bool isSaveable = false;
                    if (token.StartsWith("SampleFile:"))
                    {
                        string fileName = token.Substring("SampleFile:".Length);
                        file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{SamplesFolderName}/{fileName}"));
                    }
                    else
                    {
                        file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                        isSaveable = true;
                    }

                    if (file != null)
                    {
                        documents.Add(await LoadFromFileAsync(file, token, isSaveable));
                    }
                }
                catch { }
            }

            try
            {
                var samplesFolder = await Package.Current.InstalledLocation.GetFolderAsync(SamplesFolderName);
                foreach (var file in await samplesFolder.GetFilesAsync())
                {
                    if (file.FileType.ToLower().Equals(".json"))
                    {
                        AddActivityItems.Add(new AddUserActivityListItem()
                        {
                            DisplayName = file.Name,
                            File = file
                        });
                    }
                }
            }
            catch { }

            foreach (var doc in documents)
            {
                AddActivity(doc);
            }

            var currDoc = documents.FirstOrDefault(i => i.Token == _currentActivityToken.Value);
            if (currDoc == null)
            {
                currDoc = documents.FirstOrDefault();
            }

            CurrentActivity = currDoc;
        }

        protected string SamplesFolderName => "Samples";

        public async void CloseActivity(UserActivityViewModel document)
        {
            // TODO: Check if not saved
            int index = OpenActivities.IndexOf(document);
            if (index != -1)
            {
                OpenActivities.RemoveAt(index);

                if (index < OpenActivities.Count)
                {
                    CurrentActivity = OpenActivities[index];
                }
                else
                {
                    CurrentActivity = OpenActivities.LastOrDefault();
                }

                if (document.Token != null)
                {
                    await SaveFileTokensAsync();
                }
            }
        }

        private async Task SaveFileTokensAsync()
        {
            await Settings.SaveFileTokensAsync(OpenActivities.Select(i => i.Token));
        }
    }
}
