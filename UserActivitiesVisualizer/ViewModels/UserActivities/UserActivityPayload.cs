using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivityPayload : BindableBase
    {
        public Uri FallbackUri { get; set; }
        //
        // Summary:
        //     Gets and sets the content Uniform Resource Identifier (URI) of the image that
        //     will be used to represent the activity on another device.
        //
        // Returns:
        //     The content URI.
        public Uri ContentUri { get; set; }

        private string _contentType = "";
        //
        // Summary:
        //     Gets and sets the MIME (Multipurpose Internet Mail Extensions) type of the content
        //     stored at UserActivity.ContentUri. For example, "text/plain".
        //
        // Returns:
        //     The content type.
        public string ContentType
        {
            get => _contentType;
            set => SetProperty(ref _contentType, value);
        }
        //
        // Summary:
        //     Gets and sets the activation Uniform Resource Identifier (URI).
        //
        // Returns:
        //     The activation URI.
        public Uri ActivationUri { get; set; }
        //
        // Summary:
        //     Gets information that can be used for the details tile for this activity.
        //
        // Returns:
        //     The description, icon, and so on, associated with this **UserActivity**.
        public UserActivityVisualElementsPayload VisualElements { get; } = new UserActivityVisualElementsPayload();
        //
        // Summary:
        //     Gets and sets whether the particular activity's metadata should be uploaded to
        //     the Microsoft Cloud.
        //
        // Returns:
        //     The boolean representing whether metadata should uploaded. If not set, this property
        //     defaults to True.
        public bool IsRoamable { get; set; }

        public string ToJson()
        {
            return "TODO";
        }

        public static UserActivityPayload FromJson(string json)
        {
            return new UserActivityPayload();
        }
    }
}
