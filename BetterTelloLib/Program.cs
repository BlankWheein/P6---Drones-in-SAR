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
while (true)
{
    var string_ = Console.ReadLine();
    if (string_.Contains("t"))
    {
        Console.WriteLine($"{string_.Replace("t", "")}: {commander.State.ExtTof}");
    }
    Task.Delay(5);
}

commander.Dispose();

