using System.Windows;
using EclipsePlugInRunner.Scripting;

namespace PlanCheck.Runner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            ScriptRunner.Run(new VMS.TPS.Script());
        }
    }
}
