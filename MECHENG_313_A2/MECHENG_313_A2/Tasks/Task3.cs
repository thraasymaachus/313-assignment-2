using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;

namespace MECHENG_313_A2.Tasks
{
    //Constructs two related objects

    internal class Task3 : Task2
    {
        public override TaskNumber TaskNumber => TaskNumber.Task3;
        private int redlength = 1000, greenlength = 1000;
        private bool wantEnter = false;
        private bool allowedEnter = false;
        private int delayTime;
        private static System.Timers.Timer trafficTimer = new System.Timers.Timer(1000);


        public override void ConfigLightLength(int redLength, int greenLength)
        {
            //Alter time delays for red and green light
            this.redlength = redLength;
            this.greenlength = greenLength;
        }

        public override void ExitConfigMode()
        {
            //Set the delay immediately after exitingm in order to apply new interval
            base.ExitConfigMode();
            SetDelay();
        }

        public override async Task<bool> EnterConfigMode()
        {
            //Toogle state to want enter
            wantEnter = true;

            //Wait for the enter is allowed by Tickand then return true
            await Task.Run(() =>
            {
                while (allowedEnter == false)
                { }
            });
            allowedEnter = false;

            return true;
        }

        public override async Task Start()
        {
            await base.Start();
            //Setting up timer to autotick
            trafficTimer.Elapsed += OnTimeEvent;
            trafficTimer.Enabled = true;
            trafficTimer.AutoReset = true;
        }

        private void OnTimeEvent(Object sender, ElapsedEventArgs e)
        {
            Tick();
            SetDelay();
        }

        public void SetDelay()
        {
            //Adjust timer delaying time (Traffic light display time) based on current state
            string currentstate = getCurrentstate();
            if (currentstate == "G")
            {
                delayTime = greenlength;
            }
            else if (currentstate == "R")
            {
                delayTime = redlength;
            }
            else
            {
                delayTime = 1000;
            }

            trafficTimer.Interval = delayTime;
        }

        public override void Tick()
        {
            //Enter config mode if user want enter and currently at end of red light
            //Otherwise do normal tick
            if (wantEnter && (getCurrentstate() == "R"))
            {
                _ = base.EnterConfigMode();
                allowedEnter = true;
                wantEnter = false;
            }
            else
            {
                base.Tick();
            }
        }
    }
}
