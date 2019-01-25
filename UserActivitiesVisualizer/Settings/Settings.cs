using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI;

namespace UserActivitiesVisualizer.Settings
{
    public abstract class PayloadEditorSettings
    {
        public async Task<string[]> GetFileTokensAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(OpenTokensFileName);
                return (await FileIO.ReadLinesAsync(file)).ToArray();
            }
            catch { return new string[0]; }
        }

        public async Task SaveFileTokensAsync(IEnumerable<string> tokens)
        {
            try
            {
                tokens = tokens.Where(i => i != null).ToArray();
                var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(OpenTokensFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, tokens);
            }
            catch { }
        }

        protected abstract string OpenTokensFileName { get; }

        protected static void SetFolder(string token, StorageFolder folder)
        {
            if (folder == null)
            {
                StorageApplicationPermissions.FutureAccessList.Remove(token);
                return;
            }

            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, folder);
        }

        protected static void SetFile(string token, StorageFile file)
        {
            if (file == null)
            {
                StorageApplicationPermissions.FutureAccessList.Remove(token);
                return;
            }

            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, file);
        }
    }

    public class UserActivityPayloadEditorSettings : PayloadEditorSettings
    {
        protected override string OpenTokensFileName => "UserActivityFiles.dat";
    }

    public sealed class Settings
    {
        internal static readonly string XML_FOLDER_TOKEN = "XmlFolderToken";



        internal static readonly string DISPLAY_NAME = "DisplayName";
        internal static readonly string COLOR = "Color";


        internal static readonly string CURRENT_FILE_TOKEN = "CurrentFileToken";


        internal static readonly string TILE_SQUARE_44x44_LOGO = "TileSquare44x44Logo";
        internal static readonly string TILE_SQUARE_71x71_LOGO = "TileSquare71x71Logo";
        internal static readonly string TILE_SQUARE_150x150_LOGO = "TileSquare150x150Logo";
        internal static readonly string TILE_SQUARE_310x310_LOGO = "TileSquare310x310Logo";
        internal static readonly string TILE_WIDE_310x150_LOGO = "TileWide310x150Logo";

        internal static readonly string KEY_SHOW_NAME_ON_SQUARE_150x150_LOGO = "ShowNameOnSquare150x150Logo";
        internal static readonly string KEY_SHOW_NAME_ON_WIDE_310x150_LOGO = "ShowNameOnWide310x150Logo";
        internal static readonly string KEY_SHOW_NAME_ON_SQUARE_310x310_LOGO = "ShowNameOnSquare310x310Logo";

        internal const string KEY_OS_BUILD_NUMBER = "OSBuildNumber";

        internal const string KEY_STORED_SELECTED_MENU_ITEM = "StoredSelectedMenuItem";

        internal static readonly Uri DEFAULT_TILE_SQUARE_44x44_LOGO = new Uri("ms-appx:///Assets/DefaultTile/SmallLogo.png");
        internal static readonly Uri DEFAULT_TILE_SQUARE_71x71_LOGO = new Uri("ms-appx:///Assets/DefaultTile/Logo.png");
        internal static readonly Uri DEFAULT_TILE_SQUARE_150x150_LOGO = new Uri("ms-appx:///Assets/DefaultTile/Logo.png");
        internal static readonly Uri DEFAULT_TILE_WIDE_310x150_LOGO = new Uri("ms-appx:///Assets/DefaultTile/WideLogo.png");
        internal static readonly Uri DEFAULT_TILE_SQUARE_310x310_LOGO = new Uri("ms-appx:///Assets/DefaultTile/Logo.png");

        internal static readonly string SAMPLES_FOLDER_PATH = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Samples";

        internal static IPropertySet _values = ApplicationData.Current.LocalSettings.CreateContainer("Settings", ApplicationDataCreateDisposition.Always).Values;

        public static readonly PayloadEditorSettings UserActivityPayloadEditorSettings = new UserActivityPayloadEditorSettings();



        private static SettingsValueHelper<string> _displayName = new SettingsValueHelper<string>(DISPLAY_NAME, "Preview");
        private static SettingsValueHelper<byte[]> _color = new SettingsValueHelper<byte[]>(COLOR, ColorToByteArray(Color.FromArgb(255, 0, 120, 215)));

        private static SettingsValueHelper<string> _tileSquare44x44Logo = new SettingsValueHelper<string>(TILE_SQUARE_44x44_LOGO, DEFAULT_TILE_SQUARE_44x44_LOGO.OriginalString);
        private static SettingsValueHelper<string> _tileSquare71x71Logo = new SettingsValueHelper<string>(TILE_SQUARE_71x71_LOGO, DEFAULT_TILE_SQUARE_71x71_LOGO.OriginalString);
        private static SettingsValueHelper<string> _tileSquare150x150Logo = new SettingsValueHelper<string>(TILE_SQUARE_150x150_LOGO, DEFAULT_TILE_SQUARE_150x150_LOGO.OriginalString);
        private static SettingsValueHelper<string> _tileSquare310x310Logo = new SettingsValueHelper<string>(TILE_SQUARE_310x310_LOGO, DEFAULT_TILE_SQUARE_310x310_LOGO.OriginalString);
        private static SettingsValueHelper<string> _tileWide310x150Logo = new SettingsValueHelper<string>(TILE_WIDE_310x150_LOGO, DEFAULT_TILE_WIDE_310x150_LOGO.OriginalString);

        private static SettingsValueHelper<bool> _showNameOnSquare150x150Logo = new SettingsValueHelper<bool>(KEY_SHOW_NAME_ON_SQUARE_150x150_LOGO, false);
        private static SettingsValueHelper<bool> _showNameOnWide310x150Logo = new SettingsValueHelper<bool>(KEY_SHOW_NAME_ON_WIDE_310x150_LOGO, false);
        private static SettingsValueHelper<bool> _showNameOnSquare310x310Logo = new SettingsValueHelper<bool>(KEY_SHOW_NAME_ON_SQUARE_310x310_LOGO, false);

        private static SettingsValueHelper<int?> _osBuildNumber = new SettingsValueHelper<int?>(KEY_OS_BUILD_NUMBER, null);

        private static SettingsValueHelper<StoredSelectedMenuItem> _storedSelectedMenuItem = new SettingsValueHelper<StoredSelectedMenuItem>(KEY_STORED_SELECTED_MENU_ITEM, StoredSelectedMenuItem.Toast);

        public static Uri TileSquare44x44Logo
        {
            get { return StringToUri(_tileSquare44x44Logo.Value); }
            set { _tileSquare44x44Logo.Value = UriToString(value); }
        }

        public static Uri TileSquare71x71Logo
        {
            get { return StringToUri(_tileSquare71x71Logo.Value); }
            set { _tileSquare71x71Logo.Value = UriToString(value); }
        }

        public static Uri TileSquare150x150Logo
        {
            get { return StringToUri(_tileSquare150x150Logo.Value); }
            set { _tileSquare150x150Logo.Value = UriToString(value); }
        }

        public static Uri TileSquare310x310Logo
        {
            get { return StringToUri(_tileSquare310x310Logo.Value); }
            set { _tileSquare310x310Logo.Value = UriToString(value); }
        }

        public static Uri TileWide310x150Logo
        {
            get { return StringToUri(_tileWide310x150Logo.Value); }
            set { _tileWide310x150Logo.Value = UriToString(value); }
        }

        public static string DisplayName
        {
            get { return _displayName.Value; }
            set { _displayName.Value = value; }
        }

        public static Color Color
        {
            get { return ByteArrayToColor(_color.Value); }
            set { _color.Value = ColorToByteArray(value); }
        }

        public static bool ShowNameOnSquare150x150Logo
        {
            get { return _showNameOnSquare150x150Logo.Value; }
            set { _showNameOnSquare150x150Logo.Value = value; }
        }

        public static bool ShowNameOnWide310x150Logo
        {
            get { return _showNameOnWide310x150Logo.Value; }
            set { _showNameOnWide310x150Logo.Value = value; }
        }

        public static bool ShowNameOnSquare310x310Logo
        {
            get { return _showNameOnSquare310x310Logo.Value; }
            set { _showNameOnSquare310x310Logo.Value = value; }
        }




        public static int? OSBuildNumber
        {
            get { return _osBuildNumber.Value; }
            set { _osBuildNumber.Value = value; }
        }

        public static StoredSelectedMenuItem StoredSelectedMenuItem
        {
            get { return _storedSelectedMenuItem.Value; }
            set { _storedSelectedMenuItem.Value = value; }
        }

        private static Color ByteArrayToColor(byte[] b)
        {
            return Color.FromArgb(b[0], b[1], b[2], b[3]);
        }

        private static byte[] ColorToByteArray(Color c)
        {
            return new byte[]
            {
                c.A,
                c.R,
                c.G,
                c.B
            };
        }

        private static string UriToString(Uri uri)
        {
            return uri.ToString();
        }

        private static Uri StringToUri(string s)
        {
            return new Uri(s);
        }
    }

    public enum StoredSelectedMenuItem
    {
        Tile,
        Toast
    }

    public sealed class SettingsValueHelper<T>
    {
        public string Key { get; private set; }
        private T _defaultValue;

        public SettingsValueHelper(string key, T defaultValue)
        {
            Key = key;
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get
            {
                object value;

                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    if (Settings._values.TryGetValue(Key, out value) && value is string)
                    {
                        try
                        {
                            return (T)Enum.Parse(typeof(T), value as string);
                        }
                        catch { return _defaultValue; }
                    }
                    return _defaultValue;
                }

                if (Settings._values.TryGetValue(Key, out value) && value is T)
                    return (T)value;

                return _defaultValue;
            }

            set
            {
                if (typeof(T).GetTypeInfo().IsEnum)
                {
                    Settings._values[Key] = value.ToString();
                }
                else
                {
                    Settings._values[Key] = value;
                }
            }
        }
    }
}
