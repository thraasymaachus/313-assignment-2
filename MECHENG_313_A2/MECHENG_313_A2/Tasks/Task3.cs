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
        private int redlength=1000, greenlength=1000;
        private bool canenter = false;
        private bool allowedenter = false;
        private bool wantenter = false;
        private int delayTime;
        private static System.Timers.Timer trafficTimer = new System.Timers.Timer(1000);
        

        public override void ConfigLightLength(int redLength, int greenLength)
        {
            this.redlength = redLength;
            this.greenlength = greenLength;

            

        }

        public override void ExitConfigMode()
        {
            base.ExitConfigMode();

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

        public override async Task<bool> EnterConfigMode()
        {
            canenter = true;
            await Task.Run(() =>
            {
 
            while (allowedenter == false)
                {
                    Task.Delay(100).Wait();

                }
            });
            allowedenter = false;

            return true;
        }

        public override async Task Start()
        {
            await base.Start();
            
            trafficTimer.Elapsed += OnTimeEvent;
            trafficTimer.Enabled = true;
            trafficTimer.AutoReset = true;
        }

        private void OnTimeEvent(Object sender, ElapsedEventArgs e)
        {
            Tick();
            SetDelay();
            //if ((currentstate == "CY") || (currentstate == "CB")) {
            //    SetDelay();
            //    Tick();
            //}
            //else
            //{
            //    Tick();
            //    SetDelay();
            //}
        }

        public void SetDelay()
        {
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
            if (canenter&&(getCurrentstate()=="R"))
            {
                _=base.EnterConfigMode();
                allowedenter = true;
                canenter = false;
            }
            else
            {
                base.Tick();
            }
        }
    }
}
