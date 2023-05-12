using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{
    
    public class FiniteStateMachine : IFiniteStateMachine
    {
        private string currentState;
        private Dictionary<string, Dictionary<string, StateInformation>> fst = new Dictionary<string, Dictionary<string, StateInformation>>();

        public struct StateInformation // Define a struct to represent our state/events
        {
            public string next;
            public List<TimestampedAction> actions;

            public StateInformation(string nextState, List<TimestampedAction> actionsList)
            {
                next = nextState;
                actions = actionsList;
            }
        }

        public void AddAction(string state, string eventTrigger, TimestampedAction action) // Implement 
        {
            if (!fst.ContainsKey(state)) // Check if the FST has the state
            {
                fst.Add(state, new Dictionary<string, StateInformation>()); // If it doesn't, add the row
            }

            if (!fst[state].ContainsKey(eventTrigger)) // Check if the FST doesn't yet have the event within the state. If the FST previously had no state, this will necessarily be true.
            {
                fst[state].Add(eventTrigger, new StateInformation("", new List<TimestampedAction>())); // If it doesn't, add the column. Note that the default value for nextState is ""
            }


            StateInformation updatedStateInfo = fst[state][eventTrigger]; // Need to use a temp struct, otherwise we will be working on a copy instead of the actual variable
            updatedStateInfo.actions.Add(action);
            fst[state][eventTrigger] = updatedStateInfo; // Add the action to the list of actions associated with the action/event
        }

        public string GetCurrentState()
        {
            return currentState;
        }

        public string ProcessEvent(string eventTrigger)
        {
            System.Diagnostics.Debug.WriteLine($"Processing event: {eventTrigger}");
            
            string newState = fst[currentState][eventTrigger].next; // Figure out the next state, by accessing the FST

            System.Diagnostics.Debug.WriteLine($"New state: {eventTrigger}");

            StateInformation newStateInformation = fst[newState][eventTrigger];
            // From there, get the list of actions
            List<TimestampedAction> actions = newStateInformation.actions;

            // Initialise a ThreadPool? Maybe there's an existing one associated with the FiniteStateMachine? (There's probably not meant to be an external external one, cause nothing ThreadPool related is passed into ProcessEvent())
            
            foreach(TimestampedAction action in actions) // For each of the actions,
            {
                Thread actionThread = new Thread(() => action(DateTime.Now)); // Associate a worker thread with the action
                actionThread.Start(); // Start the thread
            }
            // Need to make threads anonymous? Perhaps use a ThreadPool? TBD

            actionThread.join();

            return null; // Return the next state
        }

        public void SetCurrentState(string state)
        {
            currentState = state;
        }

        public void SetNextState(string state, string nextState, string eventTrigger)
        {
            if (!fst.ContainsKey(state)) // Check if the FST has the state
            {
                fst.Add(state, new Dictionary<string, StateInformation>()); // If it doesn't, add the row
            }

            if (!fst[state].ContainsKey(eventTrigger)) // Check if the FST doesn't yet have the event within the state. If the FST previously had no state, this will necessarily be true.
            {
                fst[state].Add(eventTrigger, new StateInformation("", new List<TimestampedAction>())); // If it doesn't, add the column
            }

            StateInformation updatedStateInfo = fst[state][eventTrigger]; // Need to use a temp struct, otherwise we will be working on a copy instead of the actual variable
            updatedStateInfo.next = nextState;
            fst[state][eventTrigger] = updatedStateInfo; // Set the next state string associated with the action/event
        }
    }
}
