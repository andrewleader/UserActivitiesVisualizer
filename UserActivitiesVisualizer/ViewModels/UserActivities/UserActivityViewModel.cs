using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivitiesVisualizer.Helpers;
using Windows.ApplicationModel.UserActivities;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Shell;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivityViewModel : BindableBase
    {
        public event EventHandler OnRequestClose;
        public event EventHandler OnRequestSaveFileTokens;

        public string Token { get; private set; }
        public StorageFile File { get; private set; }

        private string _name = "Untitled";
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public UserActivityProperties Properties { get; private set; } = new UserActivityProperties();

        public UserActivityViewModel()
        {
        }

        private bool _isPropertiesOutdated;
        private bool _isBuildNumberOutdated;
        public void FlagPropertiesChanged()
        {
            _isPropertiesOutdated = true;
        }

        public void FlagBuildNumberChanged()
        {
            _isBuildNumberOutdated = true;
        }

        private Windows.ApplicationModel.UserActivities.UserActivity _actualUserActivity;
        public async Task InitializeAsync()
        {
            _actualUserActivity = await UserActivityChannel.GetDefault().GetOrCreateUserActivityAsync(Token);

            await UpdateActualUserActivityAsync();
        }

        private async Task UpdateActualUserActivityAsync()
        {
            _actualUserActivity.ActivationUri = Payload.ActivationUri;
            _actualUserActivity.ContentType = Payload.ContentType;
            _actualUserActivity.ContentUri = Payload.ContentUri;
            _actualUserActivity.FallbackUri = Payload.FallbackUri;
            _actualUserActivity.VisualElements.Attribution = new UserActivityAttribution()
            {
                IconUri = Payload.VisualElements.Attribution.IconUri,
                AlternateText = Payload.VisualElements.Attribution.AlternateText,
                AddImageQuery = Payload.VisualElements.Attribution.AddImageQuery
            };
            _actualUserActivity.VisualElements.AttributionDisplayText = Payload.VisualElements.AttributionDisplayText;
            _actualUserActivity.VisualElements.BackgroundColor = Payload.VisualElements.BackgroundColor;

            if (Payload.VisualElements.Content != null)
            {
                _actualUserActivity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(Payload.VisualElements.Content.ToString());
            }
            else
            {
                _actualUserActivity.VisualElements.Content = null;
            }

            _actualUserActivity.VisualElements.Description = Payload.VisualElements.Description;
            _actualUserActivity.VisualElements.DisplayText = Payload.VisualElements.DisplayText;

            await _actualUserActivity.SaveAsync();
        }

        protected void ApplyBuildNumber()
        {

        }

        protected void ApplyProperties()
        {

        }

        public bool IsOutdated { get; set; } = true;

        private bool _isUnsaved = true;
        public bool IsUnsaved
        {
            get { return _isUnsaved; }
            set { SetProperty(ref _isUnsaved, value); }
        }

        public bool IsSaveable { get; private set; }

        private bool _firstLoad = true;

        public UserActivityPayload Payload { get; private set; } = new UserActivityPayload();

        private bool _isLoading;
        /// <summary>
        /// Represents whether the system is delaying loading. Views should indicate this.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { SetProperty(ref _isLoading, value); }
        }

        public ObservableCollection<ErrorViewModel> Errors { get; private set; } = new ObservableCollection<ErrorViewModel>();

        public static async Task<UserActivityViewModel> CreateFromFileAsync(StorageFile file, string token, bool isSaveable)
        {
            UserActivityViewModel answer = (UserActivityViewModel)Activator.CreateInstance(typeof(UserActivityViewModel));
            answer.IsSaveable = isSaveable;

            await answer.LoadFromFileAsync(file, token, true);

            return answer;
        }

        protected async Task LoadFromFileAsync(StorageFile file, string token, bool assignPayloadWithoutLoading)
        {
            this.Name = file.Name;
            this.File = file;
            this.Token = token;
            this.IsUnsaved = false;

            var savedActivity = SaveableUserActivity.FromJson(await FileIO.ReadTextAsync(file));
            Payload = savedActivity.Payload;
            Properties = savedActivity.Properties;

            await InitializeAsync();
        }

        public async Task SaveAsync()
        {
            try
            {
                if (!IsSaveable)
                {
                    await SaveAsAsync();
                    return;
                }

                await FileIO.WriteTextAsync(File, new SaveableUserActivity()
                {
                    Payload = Payload,
                    Properties = Properties
                }.ToJson());

                IsUnsaved = false;
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        public async Task SaveAsAsync()
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = Name
                };

                if (IsSaveable)
                {
                    savePicker.SuggestedSaveFile = File;
                }

#if CARDS
                if (IsXml(Payload))
                {
                    savePicker.FileTypeChoices.Add("XML", new string[] { ".xml" });
                }
                else
                {
                    savePicker.FileTypeChoices.Add("JSON", new string[] { ".json" });
                }
#else
                savePicker.FileTypeChoices.Add("XML", new string[] { ".xml" });
#endif

                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    File = file;
                    Name = File.Name;
                    Token = StorageApplicationPermissions.FutureAccessList.Add(file);
                    IsSaveable = true;

                    await SaveAsync();

                    // Save file tokens since the file changed, so next time user opens app it has the correct file
                    OnRequestSaveFileTokens(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                //Microsoft.HockeyApp.HockeyClient.Current.TrackException(ex);
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        /// <summary>
        /// Closes the document
        /// </summary>
        public void Close()
        {
            OnRequestClose?.Invoke(this, new EventArgs());
        }

        public static UserActivityViewModel CreateNew()
        {
            return new UserActivityViewModel()
            {
            };
        }

        private UserActivitySession _session;
        public void TriggerRecordUserActivity()
        {
            if (_session == null)
            {
                _session = _actualUserActivity.CreateSession();
            }
        }

        public void TriggerEndUserActivity()
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }

        protected void SetSingleError(ErrorViewModel error)
        {
            MakeErrorsLike(new List<ErrorViewModel>() { error });
        }

        protected void MakeErrorsLike(List<ErrorViewModel> errors)
        {
            errors.Sort(new ParseErrorComparer());
            Errors.MakeListLike(errors);
        }

        private class ParseErrorComparer : IComparer<ErrorViewModel>
        {
            public int Compare(ErrorViewModel x, ErrorViewModel y)
            {
                int answer = ((int)x.Type).CompareTo((int)y.Type);
                if (answer != 0)
                {
                    return answer;
                }

                if (x.Position == null)
                {
                    if (y.Position == null)
                    {
                        return 0;
                    }

                    return -1;
                }
                if (y.Position == null)
                {
                    return 1;
                }

                return x.Position.LineNumber.CompareTo(y.Position.LineNumber);
            }
        }
    }
}
