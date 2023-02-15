

using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;
using TelloCommander.Interfaces;



var dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
var commander = new DroneCommander(new TelloConnection(), dictionary);
commander.Connect();

// Ask for a command to process and process it. Repeat until the an empty
// command is entered
bool isEmpty;
do
{
    Console.Write("Please enter a command or press [ENTER] to quit : ");
    string command = Console.ReadLine().Trim();
    isEmpty = string.IsNullOrEmpty(command);
    if (!isEmpty)
    {
        try
        {
            // Process the command using the API
            Console.WriteLine($"Command  : {command}");
            commander.RunCommand(command);
            Console.WriteLine($"Response : {commander.LastResponse}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
while (!isEmpty);

// Disconnect from the drone
commander.Disconnect();