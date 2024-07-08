namespace sysdiag.poc;

/// <summary>
/// Simple test application used to experiment with System.Diagnostics and process management.
/// This is a simple console application that will run for a random number of seconds and then exit, 
/// writing some information to stdout and stderr we need to capture in the launching process. 
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        bool exitWithError = false;

        if (args.Length == 1 && args[0] == "error"){
            exitWithError = true;
            Console.WriteLine("Running with error");
        }

        var random = new Random();
        var duration = random.Next(9, 30);
        Console.WriteLine($"Running for {duration} seconds total");
        for(int i =0; i < duration; i++)
        {
            Console.WriteLine($"Running for {duration - i} seconds");
            Thread.Sleep(1000);
        }

        Console.Error.WriteLine("This is an error message");

        if (exitWithError){
            Console.Error.WriteLine("stderr: Exiting with error");
            Console.WriteLine("stdout: Exiting with error");
            Environment.Exit(1);
        }

        Console.WriteLine("Done");
    }
}