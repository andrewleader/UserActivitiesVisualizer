using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivitiesVisualizer.ViewModels.UserActivities;

namespace UserActivitiesVisualizer.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        public MainPageViewModel()
        {
            Initialize();
        }

        public UserActivitiesViewModel UserActivities { get; private set; }

        private void Initialize()
        {
            try
            {
                UserActivities = new UserActivitiesViewModel();
            }
            catch { }
        }
    }
}
