using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace P4VHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        // https://stackoverflow.com/questions/793100/globally-catch-exceptions-in-a-wpf-application
        // 아직 쓸것 같진 않은데.. 일단 달아놓음
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandling();
        }
        private void SetupExceptionHandling()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
#endif
        }

        private void LogUnhandledException(Exception _exception, string _source)
        {
            string message;
            Debug.Assert(false);

            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = $"Unhandled exception in {assemblyName.Name} v{assemblyName.Version}";
                message += "\n";
                message += _exception.ToString();
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                message = $"Unhandled exception ({_source})";
                message += "\n";
                message += _exception.ToString();
                MessageBox.Show(message);
            }
        }
    }

}
