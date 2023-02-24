using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterTelloLib.Commander;
using BetterTelloLib.Commander.Events;


BetterTello commander = new();
commander.Connect();
commander.Commands.Takeoff();
while (true)
{
    Task.Delay(50);
    commander.Commands.Forward(20);
}

commander.Dispose();

