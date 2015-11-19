using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using AdiIRCAPI; // Include the API
using Nef2_Compatability;

namespace Nef2_Compatibility
{
    public class Nef2_Compatibility : IPlugin // Reference this is a plugin
    {
        private string myName = "Nef2 Compatibility";
        private string myDescription = "Makes AdiIRC more compatible with Nefarious 2 servers.";
        private string myAuthor = "Robert Whitney";
        private string myVersion = "1.0.1-dev";
        private string myEmail = "xnite@AltSociety.co"; // optional

        IPlugin nef2compat = null;
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
            myHost.OnGetData += new GetData(on_GetData); // Subscribe to raw server messages.
            myHost.NotifyUser(myName + " v" + myVersion + " by " + myAuthor + " <" + myEmail + "> loaded. Please report bugs & feature requests to https://github.com/xnite/AdiIRC-Nef2-Compatibility/issues");
        }
        public operLogin loginWindow;
        private IServer serverToOperOn;

        public void on_GetData(IServer Server, string Data, out EatData Return)
        {
            /* Process data from CHECK command */
            Match checkString = Regex.Match(Data, @"^:(.*?) (290|291|286) (.*?) :(.*)$", RegexOptions.IgnoreCase);
            if (checkString.Success)
            {
                myHost.NotifyUser(checkString.Groups[4].Value);
                Return = EatData.EatAll;
                return;
            }

            /* OPER command returns */
            Match operString = Regex.Match(Data, @"^:(.*?) (381|464|491|532) (.*?) :(.*)$", RegexOptions.IgnoreCase);
            if (operString.Success)
            {
                MessageBox.Show(operString.Groups[1] + " says: " + operString.Groups[4]);
                Return = EatData.EatAll;
                return;
            }

            /* Show OPER dialogue on not enough params for oper command */
            Match operNotEnoughParams = Regex.Match(Data, @"^:(.*?) 461 (.*?) OPER :(.*)$");
            if (operNotEnoughParams.Success)
            { 
                this.serverToOperOn = Server;
                operLogin loginWindow = new operLogin();
                loginWindow.Show();
                loginWindow.VisibleChanged += new EventHandler(run_oper_command);
                Return = EatData.EatAll;
                return;
            }

            /* If nothing else, then eat no data and our plugin goes hungry */
            Return = EatData.EatNone;
        }

        public void run_oper_command(object sender, EventArgs args)
        {
            loginWindow.Close();
            if (loginWindow.Username.Text.Length >= 1 && loginWindow.Password.Text.Length >= 1)
            {
                myHost.SendCommand(this.serverToOperOn, "OPER", loginWindow.Username.Text + " " + loginWindow.Password.Text);
            } else
            {
                MessageBox.Show("Oper attempt failed because username or password was empty!");
            }
        }

        public void Dispose()
        {
            // This is called when the plugin is unloaded
            myHost.NotifyUser("If you enjoyed " + myName + " then please consider leaving PayPal donation to xnite@xnite.me :)");
        }
    }
}
