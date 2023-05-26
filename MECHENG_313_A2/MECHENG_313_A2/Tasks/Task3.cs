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
        private static System.Timers.Timer trafficTimer;
        private int delayTime = 1000;

        public override void ConfigLightLength(int redLength, int greenLength)
        {
            this.redlength = redLength;
            this.greenlength = greenLength;

        }

        public override async Task<bool> EnterConfigMode()
        {
            wantenter = true;
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
            //var timer = new Timer(Tick, null, 0, 1000);

            // Start a background task that invokes Tick() every 1 second
            
            trafficTimer = new System.Timers.Timer(delayTime);
            trafficTimer.Elapsed += OnTimeEvent;
            trafficTimer.Enabled = true;
            trafficTimer.AutoReset = true;
        }

        private void OnTimeEvent(Object sender, ElapsedEventArgs e)
        {
            Tick();

            string currentstate = getCurrentstate();
            this.canenter = false;
            if (currentstate == "G")
            {
                delayTime = greenlength;
            }
            else if (currentstate == "R")
            {
                delayTime = redlength;
                if (wantenter)
                {
                    this.canenter = true;
                    wantenter = false;
                }
            }
            else
            {
                delayTime = 1000;
            }

            trafficTimer.Interval = delayTime;
        }


        public override void Tick()
        {
            if (canenter)
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
