using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using MagBot_FFXIV_v02.Properties;

namespace MagBot_FFXIV_v02
{
    class RouteManager //This class will eventually create a list of multiple routes. For now load one route with static LoadFile
    {
        private static readonly RouteManager RouteManagerInstance = new RouteManager();
        static RouteManager() { }
        private RouteManager()
        {
            Routes = new List<Route>();
        }
        public static RouteManager Instance
        {
            get { return RouteManagerInstance; }
        }

        public readonly List<Route> Routes;

        public void LoadRouteManager(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //throw new FileNotFoundException("Could not find the specified file!", filePath);
                MessageBox.Show(Resources.RouteManager_RouteManager_File_not_found___,
        Resources.RouteManager_RouteManager_Error,
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation,
        MessageBoxDefaultButton.Button1);
                return;
            }

            Routes.Clear();
            
            XElement file = XElement.Load(filePath);

            IEnumerable<XElement> xroutes = file.Descendants("Route");
            foreach (XElement xroute in xroutes)
            {
                var route = new Route(xroute);
                Routes.Add(route);
            }
        }

        public void Save(string filePath)
        {
            var file = new XElement("RouteManager");

            foreach (Route route in Routes)
                file.Add(route.GetXml());

            try
            {
                file.Save(filePath);
                MessageBox.Show(Resources.RouteManager_Save_Saved_as__ + filePath,
        Resources.RouteManager_Save_File_Saved,
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation,
        MessageBoxDefaultButton.Button1);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(filePath + Resources.RouteManager_Save__not_found__File_not_saved___,
        Resources.RouteManager_RouteManager_Error,
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation,
        MessageBoxDefaultButton.Button1);
            }
        }

    }
}
