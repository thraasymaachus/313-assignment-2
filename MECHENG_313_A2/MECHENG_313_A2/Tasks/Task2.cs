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
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SetLogEntries.txt");

        public Task2()
        {
            //The constructer that set the FSM states, events, actions and next states
            FSM.SetNextState("G", "Y", "a");
            FSM.AddAction("G", "a", SendToMC);
            FSM.AddAction("G", "a", WriteToLog);
            FSM.AddAction("G", "a", UpdateGUI);
            FSM.SetNextState("G", "", "b");

            FSM.SetNextState("Y", "R", "a");
            FSM.AddAction("Y", "a", SendToMC);
            FSM.AddAction("Y", "a", WriteToLog);
            FSM.AddAction("Y", "a", UpdateGUI);
            FSM.SetNextState("Y", "", "b");

            FSM.SetNextState("R", "G", "a");
            FSM.AddAction("R", "a", SendToMC);
            FSM.AddAction("R", "a", WriteToLog);
            FSM.AddAction("R", "a", UpdateGUI);
            FSM.SetNextState("R", "CY", "b");
            FSM.AddAction("R", "b", SendToMC);
            FSM.AddAction("R", "b", WriteToLog);
            FSM.AddAction("R", "b", UpdateGUI);

            FSM.SetNextState("CY", "CB", "a");
            FSM.AddAction("CY", "a", SendToMC);
            FSM.AddAction("CY", "a", WriteToLog);
            FSM.AddAction("CY", "a", UpdateGUI);
            FSM.SetNextState("CY", "R", "b");
            FSM.AddAction("CY", "b", SendToMC);
            FSM.AddAction("CY", "b", WriteToLog);
            FSM.AddAction("CY", "b", UpdateGUI);

            FSM.SetNextState("CB", "CY", "a");
            FSM.AddAction("CB", "a", SendToMC);
            FSM.AddAction("CB", "a", WriteToLog);
            FSM.AddAction("CB", "a", UpdateGUI);
            FSM.SetNextState("CB", "R", "b");
            FSM.AddAction("CB", "b", SendToMC);
            FSM.AddAction("CB", "b", WriteToLog);
            FSM.AddAction("CB", "b", UpdateGUI);
        }

        public virtual void ConfigLightLength(int redLength, int greenLength) { }

        public string getCurrentstate()
        {
            return FSM.GetCurrentState();
        }

        public virtual async Task<bool> EnterConfigMode()
        {
            //Process the config triggered event "b" and enter config mode
            string message = $"{DateTime.Now.ToString()}\tEventTriggered\tConfig\tConfig event triggered";
            WriteInFile(message);

            string nextstate = FSM.ProcessEvent("b");

            //Set state and return true only if current state is red. I.e. nextstate are not empty
            if (nextstate == "")
            {
                return false;
            }
            else
            {
                FSM.SetCurrentState(nextstate);
                return true;
            }

        }

        public void WriteInFile(string message)
        {
            //write log to file
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            //Update GUI log
            _taskPage.AddLogEntry(message);
        }

        public virtual void ExitConfigMode()
        {
            //Process the config triggered event "b" and leave config mode
            string message = $"{DateTime.Now.ToString("o")}\tEventTriggered\tConfig\tConfig event triggered";
            WriteInFile(message);
            string nextState = FSM.ProcessEvent("b");
            FSM.SetCurrentState(nextState);
        }

        public async Task<string[]> GetPortNames()
        {
            //Find Available Ports
            return await MSI.GetPortNames();
        }

        public async Task<string> OpenLogFile()
        {
            //Check if file exists, if yes read existing content, if not create it
            if (File.Exists(filePath))
            {
                string[] text = File.ReadAllLines(filePath);
                Array.Reverse(text);
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
            //Open selected port

            bool isopen = await MSI.OpenPort(serialPort, baudRate);

            //Initlitialize a delegate
            SerialReadAction readAction = (timestamp, serialInput) =>
            {
                //Invoke the function call with parameters from MockSerialInterface
                _taskPage.SerialPrint(DateTime.Now, $"{serialInput}\n");
            };
            //Add delegate to event
            MSI.SerialDataReceived += readAction;
            return isopen;
        }

        public void RegisterTaskPage(ITaskPage taskPage)
        {
            _taskPage = taskPage;
        }

        public virtual async Task Start()
        {
            //Start the Systen 
            await MSI.SetState(TrafficLightState.Red);
            FSM.SetCurrentState("R");
            //Log the start
            string message = $"{DateTime.Now.ToString()}\tProgram Start";
            WriteInFile(message);
            Tick();
        }

        public virtual void Tick()
        {
            //Log the tick
            string message = $"{DateTime.Now.ToString()}\tEventTriggered\tTick\tTick event triggered";
            WriteInFile(message);
            //Process the tick triggered event "a"
            string nextstate = FSM.ProcessEvent("a");
            FSM.SetCurrentState(nextstate);
        }

        public TrafficLightState MatchEnumFinite(string state)
        {
            //This function match the states of Finite state machine with the enum TrafficLightState
            if (state == "G")
            {
                return TrafficLightState.Green;
            }
            else if (state == "R")
            {
                return TrafficLightState.Red;
            }
            else if (state == "Y")
            {
                return TrafficLightState.Yellow;
            }
            else if (state == "CY")
            {
                return TrafficLightState.Yellow;
            }
            else
            {
                return TrafficLightState.None;
            }
        }


        public async void SendToMC(DateTime timestamp)
        {
            //Send a serial command to the Microcontroller
            string nextstate = FSM.GetNextState();
            TrafficLightState state = MatchEnumFinite(nextstate);
            string serialResponse = await MSI.SetState(state);
            //Show the Serial Response
            _taskPage.SerialPrint(DateTime.Now, $"{serialResponse}\n");
        }

        public void WriteToLog(DateTime timestamp)
        {
            //update the log
            string nextstate = FSM.GetNextState();
            TrafficLightState state = MatchEnumFinite(nextstate);
            string message = $"{DateTime.Now.ToString()}\tStateEntered\t{nextstate}\tEntered next state {nextstate}";
            WriteInFile(message);
        }

        public void UpdateGUI(DateTime timestamp)
        {
            //Update the GUI and update the log
            string nextstate = FSM.GetNextState();
            TrafficLightState state = MatchEnumFinite(nextstate);
            _taskPage.SetTrafficLightState(state);
            string message = $"{DateTime.Now.ToString()}\tActionFinished\tUpdated GUI light\tThe GUI traffic light has been set to {state.ToString()}";
            WriteInFile(message);
        }
    }
}
