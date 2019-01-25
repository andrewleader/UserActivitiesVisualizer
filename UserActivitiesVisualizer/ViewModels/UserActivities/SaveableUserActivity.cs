using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivitiesVisualizer.ViewModels.UserActivities
{
    public class SaveableUserActivity
    {
        public UserActivityProperties Properties { get; set; }

        public UserActivityPayload Payload { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SaveableUserActivity FromJson(string json)
        {
            SaveableUserActivity answer;
            try
            {
                answer = JsonConvert.DeserializeObject<SaveableUserActivity>(json);
            }
            catch { answer = new SaveableUserActivity(); }
            if (answer.Properties == null)
            {
                answer.Properties = new UserActivityProperties();
            }
            if (answer.Payload == null)
            {
                answer.Payload = new UserActivityPayload();
            }
            return answer;
        }
    }
}
