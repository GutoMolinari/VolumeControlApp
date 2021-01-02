using EventHook;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using VolumeAppTest;

namespace VolumeAppFormTests
{
    public partial class TestB : Form
    {
        private TextBox Log;
        private ForegroundTracker ex;

        public TestB()
        {
            InitializeComponent();

            Log = new DataGridTextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(Log);
            this.FormClosing += TestB_FormClosing;
            EventHookLibTest();
        }

        private void TestB_FormClosing(object sender, FormClosingEventArgs e)
        {
            applicationWatcher.Stop();
        }

        ApplicationWatcher applicationWatcher;

        public void EventHookLibTest()
        {
            using (var eventHookFactory = new EventHookFactory())
            {
                applicationWatcher = eventHookFactory.GetApplicationWatcher();
                applicationWatcher.Start();
                applicationWatcher.OnApplicationWindowChange += (s, e) =>
                {                    
                    Debug.WriteLine($"{e.ApplicationData.HWnd} - {e.ApplicationData.AppName} | {e.ApplicationData.AppTitle} | {e.Event} {Environment.NewLine}------");
                    //Console.WriteLine(string.Format("Application window of '{0}' with the title '{1}' was {2}", e.ApplicationData.AppName, e.ApplicationData.AppTitle, e.Event));
                };
            }
        }
    }
}
