using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using AdiIRCAPI; // Include the API

namespace Nef2_Compatibility
{
    public class Nef2_Compatibility : IPlugin // Reference this is a plugin
    {
        private string myName = "Nef2 Compatibility";
        private string myDescription = "Makes AdiIRC more compatible with Nefarious 2 servers.";
        private string myAuthor = "Robert Whitney";
        private string myVersion = "1.0.0";
        private string myEmail = "xnite@xnite.me"; // optional

        IPluginHost myHost = null;
        ITools myTools = null;

        public string Description
        {
            get { return myDescription; }
        }

        public string Author
        {
            get { return myAuthor; }
        }

        public string Name
        {
            get { return myName; }
        }

        public string Version
        {
            get { return myVersion; }
        }

        public string Email
        {
            get { return myEmail; }
        }

        public IPluginHost Host
        {
            get { return myHost; }
            set { myHost = value; }
        }

        public ITools Tools
        {
            get { return myTools; }
            set { myTools = value; }
        }

        public Nef2_Compatibility()
        {

        }

        public void Initialize()
        {
            // This is called when the plugin is loaded
            // Suscribe to delegates here
            myHost.OnGetData += new GetData(on_GetData);
            myHost.NotifyUser(myName + " v" + myVersion + " by " + myAuthor + "<" + myEmail + "> loaded. Please report bugs to https://github.com/xnite/");
        }

        public void on_GetData(IServer Server, string Data, out EatData Return)
        {
            Match checkString = Regex.Match(Data, @"^:(.*?) (290|291|286) (.*?) :(.*)$", RegexOptions.IgnoreCase);
            if (checkString.Success)
            {
                myHost.NotifyUser(checkString.Groups[4].Value);
                Return = EatData.EatAll;
            }
            else
            {
                Return = EatData.EatNone;
            }
        }

        public void Dispose()
        {
            // This is called when the plugin is unloaded
            MessageBox.Show("Bye world");
        }
    }
}
