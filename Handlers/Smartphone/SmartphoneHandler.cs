using Backend.Models.SmartphoneModel.Apps;
using Backend.Models.SmartphoneModel.Apps.Data;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Handlers.Smartphone
{
    public class SmartphoneHandler : Script
    {
        public SmartphoneHandler()
        {
            InitializeApps();
        }

        public List<AppModel> Apps = new List<AppModel>();
        public void InitializeApps()
        {
            Apps.Add(new PhoneApp(1));
            Apps.Add(new MessangerApp(2));
            Apps.Add(new NotesApp(3));
            Apps.Add(new SettingsApp(4));

            Apps.Add(new RadioApp(5));
            Apps.Add(new LifeinvaderApp(6));
            Apps.Add(new FraktionsApp(7));
            Apps.Add(new DispatchApp(8));
        }
    }
}
