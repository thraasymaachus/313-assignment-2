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
                        await Task.Delay(2000);
                    }
                    else if (currentstate == "R")
                    {
                        await Task.Delay(2000);
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                    Tick();
                }
            });
        }
    }
}
