using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterTelloLib.Commander;


BetterTello commander = new();
commander.Connect();

while (true)
{
    Task.Delay(10);
}

commander.Dispose();