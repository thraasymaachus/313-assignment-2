using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MECHENG_313_A2.Tasks
{
    internal class Task2 : IController
    {
        public virtual TaskNumber TaskNumber => TaskNumber.Task2;

        protected ITaskPage _taskPage;

        public void ConfigLightLength(int redLength, int greenLength)
        {
            // TODO: Implement this
        }

        public async Task<bool> EnterConfigMode()
        {
            // TODO: Implement this
            return false;
        }

        public void ExitConfigMode()
        {
            // TODO: Implement this
        }

        public async Task<string[]> GetPortNames()
        {
            // TODO: Implement this
            return new string[0];
        }

        public async Task<string> OpenLogFile()
        {
            // TODO: Implement this

            // Help notes: to read a file named "log.txt" under the LocalApplicationData directory,
            // you may use the following code snippet:
            // string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "log.txt");
            // string text = File.ReadAllText(filePath);
            //
            // You can also create/write to file(s) through System.IO.File. 
            // See https://learn.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/data/files?tabs=windows, and
            // https://learn.microsoft.com/en-us/dotnet/api/system.io.file?view=netstandard-2.0 for more details.
            return null;
        }

        public async Task<bool> OpenPort(string serialPort, int baudRate)
        {
            // TODO: Implement this
            return false;
        }

        public void RegisterTaskPage(ITaskPage taskPage)
        {
            _taskPage = taskPage;
        }

        public void Start()
        {
            // TODO: Implement this
        }

        public void Tick()
        {
            // TODO: Implement this
        }
    }
}
