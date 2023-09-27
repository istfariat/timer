
using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Notifications;
using ABI.System;

namespace TimerApp
{


    internal class Program
    {

        //[DllImport("User32.dll")]
        //static extern IntPtr GetForegroundWindow();

        //[DllImport("user32.dll")]
        //static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        //static private string GetActiveWindowTitle()
        //{
        //    const int nChars = 256;
        //    StringBuilder Buff = new StringBuilder(nChars);
        //    IntPtr handle = GetForegroundWindow();

        //    if (GetWindowText(handle, Buff, nChars) > 0)
        //    {
        //        string mystring = Buff.ToString();
        //        byte[] bytes = Encoding.Default.GetBytes(mystring);
        //        mystring = Encoding.UTF8.GetString(bytes);
        //        return mystring; 
        //    }
        //    return null;
        //}

        static List<string[]> history = new List<string[]>();
        static string[] fields = new string[6] { "Time started:\t", "Time ended:\t", "Duration:\t", "Field:\t\t", "Subject:\t", "Stage:\t\t" };
        static string pathToSave = "G:\\Projects\\Sharping\\Timer (cons)\\savefiles\\timerhistory.txt";
        static bool selectionActive = false;
        static int selection;
        static string[] entryField = new string[6] { "starttime", "endtime", "duration", "field", "subject", "stage" };
        //static string[] commandList = new string[7] { "start", "stop", "delete", "show", "deselect", "select", "edit"}; // first commands without args, then with 1 arg
        static Dictionary<string, string> singleCommands = new Dictionary<string, string>()
                                                                    {
                                                                        {"start", "Start a new timer." },
                                                                        {"stop", "Stop live timer, if there's any."},
                                                                        {"delete", "Delete selected entry."},
                                                                        {"show", "Show all saved entries."},
                                                                        {"deselect", "Deselect selected entry."},
                                                                        {"help", "Show help."}
                                                                    };
        static Dictionary<string, string> doubleCommands = new Dictionary<string, string>()
                                                                    { 
                                                                        {"select", "Select a timer. For use type 'select X', where X is index of chosen timer"},
                                                                        {"edit", "Edit property of selected timer. For use type 'edit X', where X is chosen property. Each timer has 'field', 'subject', 'stage', 'starttime', 'endtime', 'duration' properties.)"} 
                                                                    };

        static Stopwatch mainTimer = new Stopwatch();

        static void Main(string[] args)
        {
                        
            //while (true)
            //{
            //    Console.WriteLine("Active window:\t {0}", GetActiveWindowTitle());
            //    Thread.Sleep(2000);
            //}



            Stopwatch timer = new Stopwatch();
            //Timer inactivityTimer = new Timer();

            int i = 0;

            Console.WriteLine("This is timer, type 'start' to start a timer, type 'stop' to stop it, 'help' to see all commands.");

            LoadEntry();
            while (i < 100)
            {
                var parsedInput = ValidateCommand(InputCommand());

                if (parsedInput.Item3)
                {
                    ChooseAction(parsedInput.Item1, parsedInput.Item2);
                }

                i++;
            }

            Console.ReadLine();
        }


        static string InputCommand()
        {
            Console.Write("> ");
            return Console.ReadLine();
        }

        static (string commandName, int commandArg, bool comandValid) ValidateCommand(string userCommand)
        {
            string[] comArray = userCommand.Split(' ');
            string commandName = comArray[0];
            int commandArg = -1;
            bool commandValid = false;
            
            bool commIndexSingle = singleCommands.ContainsKey(commandName);
            //bool commIndexDouble = doubleCommands.ContainsKey(commandName);

            switch (comArray.Length)
            {
                case 1:
                    {
                        if (commIndexSingle)
                        {
                            commandValid = true;
                            break;
                        }
                        Console.WriteLine("Invalid command.");
                        break;
                    }
                case 2:
                    {
                        commandArg = Array.IndexOf(entryField, comArray[1]);

                        if (commandArg != -1 && commandName == "edit")
                        {
                            commandValid = true;
                            break;
                        }
                        else if (commandName == "select")
                        {
                            try
                            {
                                commandArg = int.Parse(comArray[1]);
                                commandValid = true;
                                break;
                            }
                            catch (ArgumentNullException)
                            {
                                Console.Write("Invalid argument. ");
                                commandValid = false;
                            }
                            catch (FormatException)
                            {
                                Console.Write("Invalid argument. ");
                                commandValid = false;
                            }
                            catch (OverflowException)
                            {
                                Console.Write("Invalid argument, number too big. ");
                                commandValid = false;
                            }
                        }

                        commandValid = false;
                        Console.WriteLine("Invalid command.");
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Invalid command.");
                        break;
                    }
            }

            return (commandName, commandArg, commandValid);
        }

        static void ChooseAction(string command, int inputIndex)
        {
            switch (command)
            {
                case "start":
                    startTimer();
                    break;
                case "stop":
                    stopTimer();
                    break;
                case "delete":
                    DeleteEntry();
                    break;
                case "show":
                    ShowHistory();
                    break;
                case "deselect":
                    DeselectEntry();
                    break;
                case "select":
                    SelectEntry(inputIndex);
                    break;
                case "edit":
                    EditEntry(inputIndex);
                    break;
                case "help":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine("Something went wrong.");
                    break;
            }
        }

        static void startTimer()
        {
            if (mainTimer.IsRunning)
            {
                Console.WriteLine("Timer already running, it is {0} currently", RoundToSeconds(mainTimer.Elapsed));
            }
            else
            {
                string[] newEntry = new string[6];

                //LoadEntry();
                history.Add(newEntry);
                mainTimer.Start();
                history[history.Count - 1][0] = DateTime.Now.ToString();
                SaveEntry(true);
            }
        }

        static void stopTimer()
        {
            if (mainTimer.IsRunning)
            {
                string roundedTime = RoundToSeconds(mainTimer.Elapsed);
                mainTimer.Stop();
                history[history.Count - 1][1] = DateTime.Now.ToString();
                history[history.Count - 1][2] = roundedTime;
                Console.WriteLine("Last entry was {0} long", roundedTime);
                mainTimer.Reset();
                SaveEntry();
            }
            else
            {
                Console.WriteLine("There is no timer runnig to stop");
            }

        }


        static void ShowHistory() //нужен ли load??
        {            
            Console.WriteLine("Here is the history:");
            for (int i = 0; i < history.Count; i++)
            {
                if (selectionActive && selection == i)
                {
                    Console.WriteLine("Timer entry {0}: (selected)", i);
                }
                else
                {
                    Console.WriteLine("Timer entry {0}:", i);
                }
                
                for (int j = 0; j < 6; j++)
                    Console.WriteLine("\t {0}{1}", fields[j], history[i][j]);
            }            
        }

        static void SelectEntry(int entryIndex)
        {
            
            if (entryIndex > history.Count - 1 || entryIndex < 0)
            {
                Console.WriteLine("There is no timer with this index.");
                selectionActive = false;
                return;
            }

            selection = entryIndex;
            selectionActive = true;
            return;
        }

        static void DeselectEntry()
        {            
            if (selectionActive)
            {
                selectionActive = false;
                Console.WriteLine("Deselection successeful.");
            }
            else
            {
                Console.WriteLine("No entry selected.");
            }
        }

        static void DeleteEntry()
        {
            if (selectionActive)
            {
                if (mainTimer.IsRunning)
                {
                    mainTimer.Stop();
                    mainTimer.Reset();
                    Console.Write("Live timer stopped. ");
                }
                history.RemoveAt(selection);
                Console.WriteLine("Entry deleted");
                DeselectEntry();                
                SaveEntry();
            }
            else
            {
                Console.WriteLine("No entry selected to delete.");
            }
        }

        static void ShowHelp()
        {
            Dictionary<string, string> allCommands = new Dictionary<string, string>();
            
            singleCommands.ToList().ForEach(x => allCommands.Add(x.Key, x.Value));
            doubleCommands.ToList().ForEach(x => allCommands.Add(x.Key, x.Value));

            foreach (var command in allCommands.OrderBy(command => command.Key))
                Console.WriteLine("{0}\t\t{1}", command.Key, command.Value);
        }

        static void EditEntry(int inputIndex)
        {
            if (!selectionActive)
                return;
            
            string timeName = "";
            
            if (inputIndex == 0)
                timeName = "starttime";
            else if (inputIndex == 1)
                timeName = "endtime";

                switch (inputIndex)
                {
                    case 0 or 1: //datetime parser
                    {
                        DateTime newTime = new DateTime();
                        DateTime startTime = new DateTime();
                        DateTime endTime = new DateTime();
                        string duration = "";
                        bool parseSuccess;

                        Console.WriteLine("Enter new {0}. Duration will be adjusted. Format of input: ", timeName);

                        (newTime, parseSuccess) = ParseStringToDateTime(InputCommand());

                        if (!parseSuccess)
                            break;

                        if (inputIndex == 0)
                        {
                            startTime = newTime;
                            endTime = ParseStringToDateTime(history[selection][1]).Item1;
                        }
                        else
                        {
                            endTime = newTime;
                            startTime = ParseStringToDateTime(history[selection][0]).Item1;
                        }

                        if (DateTime.Compare(startTime, endTime) < 0)
                        {
                            duration = endTime.Subtract(startTime).ToString("c");
                            history[selection][0] = startTime.ToString();
                            history[selection][1] = endTime.ToString();
                            history[selection][2] = duration;
                        }
                        else
                            Console.WriteLine("start time must be earlier than endtime");
                        break;
                    }
                    case 2:     //timespan parser
                    {
                        System.TimeSpan newDuration = new System.TimeSpan();
                        DateTime startTime = new DateTime();
                        DateTime endTime = new DateTime();
                        string duration = "";
                        bool parseSuccess;
                        
                        Console.WriteLine("Enter new duration of timer. Endtime will be adjusted. Format of input: hh:mm:ss", timeName);

                        (newDuration, parseSuccess) = ParseStringToTimespan(InputCommand());

                        if (!parseSuccess)
                            break;


                        (startTime, parseSuccess) = ParseStringToDateTime(history[selection][0]);

                        if (!parseSuccess)
                            break;

                        endTime = startTime.Add(newDuration);
                        
                        duration = newDuration.ToString("c");
                        history[selection][1] = endTime.ToString();
                        history[selection][2] = duration;
                        break;
                    }
                    case 3 or 4 or 5:
                    {
                        Console.WriteLine("Enter {0} of your activity:", fields[inputIndex]);
                        history[selection][inputIndex] = InputCommand();
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Something went wrong.");
                        return;
                    }
                }
            SaveEntry();
        }

        static void ParseInput(string userInput)
        {

        }
        static (DateTime, bool success) ParseStringToDateTime(string sourceString)
        {
            DateTime result = new DateTime();
            bool success;

            try
            {
                result = DateTime.Parse(sourceString);
                success = true;
            }
            catch
            {
                Console.WriteLine("Invalid time format input.");
                success = false;
            }

            return (result, success);
        }

        static (System.TimeSpan, bool success) ParseStringToTimespan(string sourceString)
        {
            System.TimeSpan result = new System.TimeSpan();
            bool success;

            try
            {
                result = System.TimeSpan.Parse(sourceString);
                success = true;
            }
            catch
            {
                Console.WriteLine("Invalid time format input.");
                success = false;
            }
            
            return (result, success);
        }

       
        static string RoundToSeconds(System.TimeSpan timeInput)
        {
            int precision = 0; // how many digits past the decimal point
            const int TIMESPAN_SIZE = 7; // it always has seven digits
                                         // convert the digitsToShow into a rounding/truncating mask
            int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));

            //TimeSpan roundedTimeSpan = new TimeSpan(((long)Math.Round((1.0 * timeInput.Ticks / factor)) * factor));
            string roundedTimeSpan = timeInput.ToString("c");

            return roundedTimeSpan.Substring(0, roundedTimeSpan.Length - 8);
        }



        static void SaveEntry (bool append = false)
        {
            if (!File.Exists(pathToSave))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(pathToSave))
                {
                    for (int i = 0; i < history.Count; i++)
                    {
                        for (int j = 0; j < 6; j++)
                            sw.Write("{0}\t", history[i][j]);
                        sw.Write("\n");
                    }
                }
            }
            else
            {                
                using (StreamWriter sw = new StreamWriter(pathToSave, append))
                {
                    for (int i = 0; i < history.Count; i++)
                    {
                        for (int j = 0; j < 6; j++)
                            sw.Write("{0}\t", history[i][j]);
                        sw.Write("\n");
                    }
                }
            }
            
        }

        static void LoadEntry()
        {
            if (!File.Exists(pathToSave))
            {
                Console.WriteLine("There is no file to load.");
            }
            else
            {
                // Open the file to read from.
                using (StreamReader sr = File.OpenText(pathToSave))
                {
                    string s;
                    

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] currentLine = s.Split("\t");
                        history.Add(currentLine);
                        //Console.WriteLine(s);
                    }
                }
            }
        }
    }
}