using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class AddUserActivityListItem
    {
        public static readonly AddUserActivityListItem NewActivity = new AddUserActivityListItem() { DisplayName = "\u2795 New activity" };
        public static readonly AddUserActivityListItem OpenActivity = new AddUserActivityListItem() { DisplayName = "\uD83D\uDCC2 Open activity" };

        public string DisplayName { get; set; }

        public StorageFile File { get; set; }
    }
}
