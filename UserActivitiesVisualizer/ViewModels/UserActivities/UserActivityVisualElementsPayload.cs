using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivityVisualElementsPayload : BindableBase
    {
        private string _displayText = "";
        //
        // Summary:
        //     Gets and sets the display text that is used for the details tile text for this
        //     **UserActivity**.
        //
        // Returns:
        //     The display text.
        public string DisplayText
        {
            get { return _displayText; }
            set { SetProperty(ref _displayText, value); }
        }

        private string _description = "";
        //
        // Summary:
        //     Gets and sets the description text that is used for the details tile for this
        //     **UserActivity**.
        //
        // Returns:
        //     The description.
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private JObject _content;
        //
        // Summary:
        //     Gets and sets the content (Adaptive Card) that is used for the details tile for this **UserActivity**
        //
        // Returns:
        //     The content of the tile.
        public JObject Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private Color _backgroundColor;
        //
        // Summary:
        //     Gets and sets the background color for the details tile for this **UserActivity**.
        //
        // Returns:
        //     The color for the background.
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        //
        // Summary:
        //     Gets or sets the visual information about a user activity.
        //
        // Returns:
        //     The visual information such as the URI for the icon, text used by screen readers,
        //     and so on.
        public UserActivityAttributionPayload Attribution { get; set; } = new UserActivityAttributionPayload();

        private string _attributionDisplayText = "";
        //
        // Summary:
        //     Set the text which is shown in the top banner of the activity card.
        //
        // Returns:
        //     The text that will appear in the top banner of the activity card.
        public string AttributionDisplayText
        {
            get => _attributionDisplayText;
            set => SetProperty(ref _attributionDisplayText, value);
        }
    }
}
