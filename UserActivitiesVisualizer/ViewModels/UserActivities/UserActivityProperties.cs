using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class UserActivityProperties : BindableBase
    {
        private string _appName = "Sample App";
        public string AppName
        {
            get => _appName;
            set => SetProperty(ref _appName, value);
        }
    }
}
