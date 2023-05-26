using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
            var tickrun = Task.Run(async () =>
            {
                while (true)
                {
                    string currentstate = getCurrentstate();
                    if (currentstate == "G")
                    {
                        this.canenter = false;
                        await Task.Delay(greenlength);
                    }
                    else if (currentstate == "R")
                    {
                        await Task.Delay(redlength);
                        if (wantenter)
                        {
                            this.canenter = true;
                            wantenter = false;
                        }
                    }
                    else
                    {
                        this.canenter = false;
                        await Task.Delay(1000);
                    }
                    Tick();
                }
            });
        }

        private void Tick(object state)
        {
            base.Tick();
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
