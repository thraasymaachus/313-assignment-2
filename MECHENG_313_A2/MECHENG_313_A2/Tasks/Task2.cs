using MECHENG_313_A2.Serial;
using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MECHENG_313_A2.Tasks
{
    internal class Task2 : IController
    {
        public virtual TaskNumber TaskNumber => TaskNumber.Task2;

        protected ITaskPage _taskPage;

        //Constructs two related objects
        private Serial.MockSerialInterface MSI = new Serial.MockSerialInterface();
        private FiniteStateMachine FSM = new FiniteStateMachine();

        //The event
        public event SerialReadAction SerialDataReceived;
        public Task2()
        {
            //The constructer that set the FSM states, events, actions and next states
            FSM.SetNextState("G","a","Y");
            FSM.AddAction("G", "a", SendToMC);
            FSM.AddAction("G", "a", WriteToLog);
            FSM.AddAction("G", "a", UpdateGUI);
            FSM.SetNextState("G", "b", "");

            FSM.SetNextState("Y", "a", "R");
            FSM.AddAction("Y", "a", SendToMC);
            FSM.AddAction("Y", "a", WriteToLog);
            FSM.AddAction("Y", "a", UpdateGUI);
            FSM.SetNextState("Y", "b", "");

            FSM.SetNextState("R", "a", "G");
            FSM.AddAction("R", "a", SendToMC);
            FSM.AddAction("R", "a", WriteToLog);
            FSM.AddAction("R", "a", UpdateGUI);
            FSM.SetNextState("R", "b", "CY");
            FSM.AddAction("R", "b", SendToMC);
            FSM.AddAction("R", "b", WriteToLog);
            FSM.AddAction("R", "b", UpdateGUI);

            FSM.SetNextState("CY", "a", "CB");
            FSM.AddAction("CY", "a", SendToMC);
            FSM.AddAction("CY", "a", WriteToLog);
            FSM.AddAction("CY", "a", UpdateGUI);
            FSM.SetNextState("CY", "b", "R");
            FSM.AddAction("CY", "b", SendToMC);
            FSM.AddAction("CY", "b", WriteToLog);
            FSM.AddAction("CY", "b", UpdateGUI);

            FSM.SetNextState("CB", "a", "CY");
            FSM.AddAction("CB", "a", SendToMC);
            FSM.AddAction("CB", "a", WriteToLog);
            FSM.AddAction("CB", "a", UpdateGUI);
            FSM.SetNextState("CB", "b", "R");
            FSM.AddAction("CB", "b", SendToMC);
            FSM.AddAction("CB", "b", WriteToLog);
            FSM.AddAction("CB", "b", UpdateGUI);
        }

        public void ConfigLightLength(int redLength, int greenLength)
        {
            // TODO: Implement this
        }

        public async Task<bool> EnterConfigMode()
        {
            string nextstate = FSM.ProcessEvent("b");
            FSM.SetCurrentState(nextstate);
            if (nextstate=="")
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public void ExitConfigMode()
        {
            string nextstate = FSM.ProcessEvent("b");
            FSM.SetCurrentState(nextstate);
        }

        public async Task<string[]> GetPortNames()
        {
            return await MSI.GetPortNames();
        }

        public async Task<string> OpenLogFile()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SetLogEntries.txt");
            //Check if file exists, if yes read existing content, if not create it
            
            if (File.Exists(filePath))
            {
                string[] text = File.ReadAllLines(filePath);
                _taskPage.SetLogEntries(text);
            }
            else
            {
                File.Create(filePath).Close();
            }
            
            return filePath;
        }

        public async Task<bool> OpenPort(string serialPort, int baudRate)
        {
            bool isopen = await MSI.OpenPort(serialPort, baudRate);
            //Register for event
            SerialReadAction act = (timestamp, serialInput) =>
            {
                _taskPage.SerialPrint(DateTime.Now, serialInput);
            };
           
            SerialDataReceived += act;
            return isopen;
        }

        public void RegisterTaskPage(ITaskPage taskPage)
        {
            _taskPage = taskPage;
        }

        public async void Start()
        {
            string currentstate = await MSI.SetState(TrafficLightState.Green);
            _taskPage.SerialPrint(DateTime.Now, currentstate);
            FSM.SetCurrentState("G");
            _taskPage.SetTrafficLightState(TrafficLightState.Green);
        }

        public void Tick()
        {
            string nextstate = FSM.ProcessEvent("a");
            FSM.SetCurrentState(nextstate);
        }

        public async void SendToMC(DateTime timestamp)
        {
            TrafficLightState state;
            string nextstate = FSM.GetNextState();
            if (nextstate == "G")
            {
                state = TrafficLightState.Green;
            }
            else if (nextstate == "R")
            {
                state = TrafficLightState.Red;
            }
            else if (nextstate == "Y")
            {
                state = TrafficLightState.Yellow;
            }
            else if (nextstate == "CY")
            {
                state = TrafficLightState.Yellow;
            }
            else
            {
                state = TrafficLightState.None;
            }
            await MSI.SetState(state);
        }

        public void WriteToLog(DateTime timestamp)
        {
            TrafficLightState state;
            string nextstate = FSM.GetNextState();
            if (nextstate == "G")
            {
                state = TrafficLightState.Green;
            }
            else if (nextstate == "R")
            {
                state = TrafficLightState.Red;
            }
            else if (nextstate == "Y")
            {
                state = TrafficLightState.Yellow;
            }
            else if (nextstate == "CY")
            {
                state = TrafficLightState.Yellow;
            }
            else
            {
                state = TrafficLightState.None;
            }
            _taskPage.SerialPrint(DateTime.Now, state.ToString());
        }

        public void UpdateGUI(DateTime timestamp)
        {
            TrafficLightState state;
            string nextstate = FSM.GetNextState();
            if (nextstate == "G")
            {
                state = TrafficLightState.Green;
            }else if(nextstate == "R")
            {
                state = TrafficLightState.Red;
            }else if(nextstate == "Y")
            {
                state = TrafficLightState.Yellow;
            }else if(nextstate == "CY")
            {
                state = TrafficLightState.Yellow;
            }else 
            {
                state = TrafficLightState.None;
            }
            _taskPage.SetTrafficLightState(state);
        }
    }
}
