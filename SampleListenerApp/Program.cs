using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SampleListenerApp
{
    class Program
    {
        private Process process;
        private TaskCompletionSource<bool> eventHandled;
        private DateTime startTime;
        private DateTime exitTime;
        private int exitCode;

        public async Task RunTask(string[] args)
        {
            eventHandled = new TaskCompletionSource<bool>();
            string path = "../../../../SampleConsoleApp/bin/Debug/net8.0/SampleConsoleApp";
            string param = "none";

            if (args != null && args.Length > 0){
                param = "error";
                WriteToConsole("Error flag has been set - client will exit with error");
            }
            
            try
            {
                WriteToConsole("Starting process: " + path);
                WriteToConsole("Process will start, run to completion and output will be provided here");

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = param,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.Exited += new EventHandler(OnProcessExit);
                startTime = DateTime.Now; // Record start time
                process.Start();
                WriteToConsole("Process started. Waiting for it to complete...");

                await eventHandled.Task;
            }
            catch (Exception ex)
            {
                WriteToConsole("Error: " + ex.Message);
            }
            finally
            {
                process?.Dispose();
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            exitTime = DateTime.Now; // Record exit time
            exitCode = process.ExitCode; // Record exit code

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            WriteToConsole(
                $"Exit time    : {exitTime}\n" +
                $"Exit code    : {exitCode}\n" +
                $"Elapsed time : {Math.Round((exitTime - startTime).TotalMilliseconds)}");

            WriteToConsole("Process ran to completion. All done now, output and error below:");

            if (exitCode != 0){
                WriteToConsole("Application exited with error. ");
            }

            Console.WriteLine("");
            Console.WriteLine("Output: " + output);
            Console.WriteLine("Error: " + error);

            eventHandled.TrySetResult(true);

            WriteToConsole("Listener completed. Exiting this application.");
        }

        public static async Task Main(string[] args)
        {
            Program p = new Program();
            await p.RunTask(args);
        }

        public static void WriteToConsole(string msg){
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
