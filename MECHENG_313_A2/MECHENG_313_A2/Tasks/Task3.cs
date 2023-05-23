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
        private bool allowenter = false;

        public override void ConfigLightLength(int redLength, int greenLength)
        {
            this.redlength = redLength;
            this.greenlength = greenLength;

        }

        public override async Task<bool> EnterConfigMode()
        {
            await Task.Run(() =>
            {
                while (canenter == false)
                {
                    Task.Delay(1).Wait();

                }
                canenter = false;
                allowenter = true;
                while (allowenter == true)
                {
                    Task.Delay(1).Wait();

                }
            });
            return true;
        }

        public override async void Start()
        {
            base.Start();

            // Start a background task that invokes Tick() every 1 second
            var tickrun = Task.Run(async () =>
            {
                while (true)
                {
                    string  currentstate = getCurrentstate();
                    if (currentstate == "G")
                    {
                        this.canenter = false;
                        await Task.Delay(greenlength);
                    }
                    else if (currentstate == "R")
                    {
                        await Task.Delay(redlength);
                        this.canenter = true;
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

        public override void Tick()
        {
            if (allowenter)
            {
                _ = base.EnterConfigMode();
            }
            else
            {
                base.Tick();
            }
        }
    }
}
