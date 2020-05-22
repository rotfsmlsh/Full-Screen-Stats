using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullScreenStats {
    class SettingsSelection {
        List<SettingObject> settingsList { get; set; }

        SettingsSelection(List<SettingObject> objects) {
            settingsList = objects;
        }

        public int getLength() {
            return settingsList.Count();
        }

        public void setSetting(String settingName, bool settingValue) {
            
        }
    }
}
