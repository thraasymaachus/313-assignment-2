﻿using MECHENG_313_A2.Serial;
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

        //The event
        public event SerialReadAction SerialDataReceived;
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

        public virtual void ConfigLightLength(int redLength, int greenLength)
        {
        }

        public string getCurrentstate()
        {
            return FSM.GetCurrentState();
        }

        public virtual async Task<bool> EnterConfigMode()
        {
            string message = $"{DateTime.Now.ToString("o")}\tEventTriggered\tConfig\tConfig event triggered";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            _taskPage.AddLogEntry(message);
            string nextstate = FSM.ProcessEvent("b");
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

        public FiniteStateMachine getFSM()
        {
            return FSM;
        }

        public virtual void ExitConfigMode()

        {
            string message = $"{DateTime.Now.ToString("o")}\tEventTriggered\tConfig\tConfig event triggered";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            string nextState = FSM.ProcessEvent("b");
            FSM.SetCurrentState("R");
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
                _taskPage.SerialPrint(DateTime.Now, $"{serialInput}\n");
            };

            SerialDataReceived += act;
            return isopen;
        }

        public void RegisterTaskPage(ITaskPage taskPage)
        {
            _taskPage = taskPage;
        }

        public virtual async Task Start()
        {
            string currentState = await MSI.SetState(TrafficLightState.Red);
            FSM.SetCurrentState("R");
            string message = $"{DateTime.Now.ToString("o")}\tProgram Start";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            _taskPage.AddLogEntry(message);
            Tick();
        }

        public virtual void Tick()
        {
            string message = $"{DateTime.Now.ToString("o")}\tEventTriggered\tTick\tTick event triggered";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            _taskPage.AddLogEntry(message);
            string nextstate = FSM.ProcessEvent("a");
            FSM.SetCurrentState(nextstate);
            message = $"{DateTime.Now.ToString("o")}\tStateEntered\t{nextstate}\tEntered next state {nextstate}";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            _taskPage.AddLogEntry(message);

        }

        public async void SendToMC(DateTime timestamp) // What does it do??
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
            string currentState = await MSI.SetState(TrafficLightState.Green);
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

            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine($"{DateTime.Now.ToString("o")}\t{state}");
                }
            }
            _taskPage.AddLogEntry($"{DateTime.Now.ToString("o")}\t{state}");
            _taskPage.SerialPrint(DateTime.Now, $"{state}\n");
        }

        public void UpdateGUI(DateTime timestamp)
        {
            TrafficLightState state;
            string colour;
            string nextstate = FSM.GetNextState();
            if (nextstate == "G")
            {
                state = TrafficLightState.Green;
                colour = "Green";
            }
            else if (nextstate == "R")
            {
                state = TrafficLightState.Red;
                colour = "Red";
            }
            else if (nextstate == "Y")
            {
                state = TrafficLightState.Yellow;
                colour = "Yellow";
            }
            else if (nextstate == "CY")
            {
                state = TrafficLightState.Yellow;
                colour = "Yellow";
            }
            else
            {
                state = TrafficLightState.None;
                colour = "None";
            }
            _taskPage.SetTrafficLightState(state);
            string message = $"{DateTime.Now.ToString("o")}\tActionFinished\tUpdated GUI light\tThe GUI traffic light has been set to {state.ToString()}";
            lock (this)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true)) // second argument 'true' to append data to the file
                {
                    writer.WriteLine(message);
                }
            }
            _taskPage.AddLogEntry(message);
        }
    }
}
