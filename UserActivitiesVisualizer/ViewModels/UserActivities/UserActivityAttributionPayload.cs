using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivityAttributionPayload : BindableBase
    {
        private Uri _iconUri;
        //
        // Summary:
        //     Get or set the Uniform Resource Identifier (URI) for the icon image.
        //
        // Returns:
        //     The URI that identifies the icon image.
        public Uri IconUri
        {
            get => _iconUri;
            set => SetProperty(ref _iconUri, value);
        }

        private string _alternateText = "";
        //
        // Summary:
        //     Get or set the text that describes the icon.
        //
        // Returns:
        //     The alternative text string.
        public string AlternateText
        {
            get => _alternateText;
            set => SetProperty(ref _alternateText, value);
        }

        private bool _addImageQuery;
        //
        // Summary:
        //     Get or set whether to allow Windows to append a query string to the image URI
        //     supplied from IconUri when retriving the image. The query string includes information
        //     that can be used to choose the ideal image based on the DPI of the display, the
        //     high contrast setting, and the user's language.
        //
        // Returns:
        //     **True** to allow windows to append a query string to the image URI; **false**
        //     otherwise.
        public bool AddImageQuery
        {
            get => _addImageQuery;
            set => SetProperty(ref _addImageQuery, value);
        }
    }
}
