
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
        static string[] commandList = new string[7] { "start", "stop", "delete", "show", "deselect", "select", "edit"}; // first commands without args, then with 1 arg


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
            int[] parsedInput = new int[3];
            

            Console.WriteLine("This is timer, type 'start' to start a timer, type 'stop' to stop it. You can edit name of running timer by typing 'name 1', 'name 2', 'name 3'.");

            while (i < 100)
            {
                //Console.WriteLine("Names of timer:\nname1: {0}\nname2: {1}\nname3: {2}", timerName[0], timerName[1], timerName[2]);
                //userInput(timer);


                parsedInput = ValidateCommand(InputCommand());
                
                ChooseAction(parsedInput);

                i++;
            }

            Console.ReadLine();
        }

        static void userInput(Stopwatch clock)
        {
            Console.Write("> ");
            string command = Console.ReadLine();
            
            if (command == "start")
                startTimer(clock);
            else if (command == "stop")
                stopTimer(clock);

            //case "name 1" or "name 2" or "name 3":
            //    if (clock.IsRunning)
            //        editName(command);
            //    else
            //        Console.WriteLine("No timer running to edit.");
            //    break;
            else if (command == "show")
            {
                Console.WriteLine("Here is the history:");
                for (int i = 0; i < history.Count; i++)
                {
                    Console.WriteLine("Timer entry {0}:", i + 1);
                    for (int j = 0; j < 6; j++)
                        Console.WriteLine("\t {0}{1}", fields[j], history[i][j]);
                }
            }
            else if (command == "save")
                SaveEntry();

            else if (editComms.Contains(command))
                EditEntry(command, selection, selectionActive);

            else if (command.Substring(0,6) == "select")
            {
                selectionActive = true;
                try
                {
                    selection = int.Parse(command.Substring(7));
                }
                catch (ArgumentOutOfRangeException)
                {
                    selection = 0;
                    Console.WriteLine("Invalid format. Try typing 'select X' where x is field to edit.");
                    selectionActive = false;
                }
                catch (FormatException)
                {
                    selection = 0;
                    Console.WriteLine("Invalid format. Try typing 'select X' where x is field to edit.");
                    selectionActive = false;
                }
                catch (OverflowException)
                {
                    selection = 0;
                    Console.WriteLine("Invalid format. Try typing 'select X' where x is field to edit.");
                    selectionActive = false;
                }     
            }
            else if (command == "deselect")
            {
                selectionActive = false;
                Console.WriteLine("Deselection successeful.");
            }
            else
            {
                Console.WriteLine("Invalid command.");
            }
            return;
        }

        static void startTimer(Stopwatch timer)
        {
            if (timer.IsRunning)
            {
                Console.WriteLine("Timer already running, it is {0} currently", RoundToSeconds(timer.Elapsed));
            }
            else
            {
                string[] newEntry = new string[6];

                LoadEntry();
                history.Add(newEntry);
                timer.Start();
                history[history.Count - 1][0] = DateTime.Now.ToString();
                SaveEntry();
            }
        }

        static void stopTimer(Stopwatch timer)
        {
            if (timer.IsRunning)
            {
                TimeSpan roundedTime = RoundToSeconds(timer.Elapsed);
                timer.Stop();
                history[history.Count - 1][1] = DateTime.Now.ToString();
                history[history.Count - 1][2] = roundedTime.ToString();
                System.Console.WriteLine("Last entry was {0} long", roundedTime);
                timer.Reset();
                SaveEntry();
            }
            else
            {
                Console.WriteLine("There is no timer runnig to stop");
            }

        }

        static TimeSpan RoundToSeconds(TimeSpan timeInput)
        {
            int precision = 0; // how many digits past the decimal point
            const int TIMESPAN_SIZE = 7; // it always has seven digits
                                         // convert the digitsToShow into a rounding/truncating mask
            int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));

            TimeSpan roundedTimeSpan = new TimeSpan(((long)Math.Round((1.0 * timeInput.Ticks / factor)) * factor));
            return roundedTimeSpan;
        }

        //static void editName(string userInput)
        //{

        //    string[] workStep = new string[3] { "field", "object", "task/stage" };
        //    int workIndex = int.Parse(userInput.Substring(5)) - 1;

        //    Console.WriteLine("Enter name of your {0} of work: ", workStep[workIndex]);
        //    history[history.Count - 1][workIndex + 3] = Console.ReadLine();

        //    return;
        //}

        static void SaveEntry ()
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
                using (StreamWriter sw = new StreamWriter(pathToSave,true))
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
                        Console.WriteLine(s);
                    }
                }
            }
        }

        static void EditEntry(string editField, int selectedItem, bool liveSelected = false)
        {
            editField = editField.Substring(5);
            if (!File.Exists(pathToSave) || liveSelected)
            {
                Console.WriteLine("There is no entry selected to edit. Select timer first.");
                return;
            }
            else if (selectedItem > history.Count - 1 || selectedItem < 0)
            {
                Console.WriteLine("There is timer with that number.");
                return;
            }

            switch (editField)
            {
                case "name 1":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
                case "name 2":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
                case "name 3":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
                case "start":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
                case "end":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
                case "duration":
                    history[selectedItem][0] = Console.ReadLine();
                    break;
            }
        }

        static string InputCommand ()
        {
            Console.Write("> ");
            return Console.ReadLine();
        }

        static int SelectItem(string[] command)
        {
            int itemIndex = int.Parse(command[1]);
            if (itemIndex > history.Count - 1 || itemIndex < 0)
            {
                Console.WriteLine("There is no timer with this index.");
                selectionActive = false;
                return 0;
            }
            
            selectionActive = true;
            return itemIndex;
        }

        static int[] ValidateCommand (string userCommand) //returns command, command argument, code of result 0=invalid, 1= single word comm, 2=double word comm
        {
            string[] comArray = userCommand.Split(' ');
            int[] result = new int[3];

            int commIndex = Array.IndexOf(commandList, comArray[0]);
            if (commIndex != -1)
            {
                switch (comArray.Length)
                {
                    case 1:
                        {
                            if (commIndex < 5)
                            {
                                result[0] = commIndex;
                                result[2] = 1;
                                return result;
                            }
                            break;
                        }
                    case 2:
                        {
                            int argIndex = Array.IndexOf(entryField, comArray[1]);
                            result[0] = commIndex;
                            result[2] = 2;

                            if (argIndex != -1 && commIndex == 6) //edit
                            {
                                result[1] = argIndex;
                                return result;
                            }
                            else if (commIndex == 5) //select
                            {
                                //int[] parsedInput = ParseInt(comArray[1]);

                                //if (parsedInput[1] == 0)

                                try
                                {
                                    result[1] = int.Parse(comArray[1]);
                                    result[2] = 2;
                                    return result;
                                }
                                catch (ArgumentNullException)
                                {
                                    Console.Write("Invalid argument.");
                                }
                                catch (FormatException)
                                {
                                    Console.Write("Invalid argument.");
                                }
                                catch (OverflowException)
                                {
                                    Console.Write("Invalid argument, number too big.");                                    
                                }                                
                            }

                            break;
                        }

                    default:
                        break;
                }
            }
            
            Console.WriteLine("Invalid command.");
            result[2] = 0;
            return result;            
        }


        //static int[] ParseInt(string argToInt)
        //{
        //    int[] result = new int[2];

        //    try
        //    {
        //        result[0] = int.Parse(argToInt);
        //        result[1] = 1;
        //    }
        //    catch (ArgumentNullException)
        //    { 
        //        Console.WriteLine("Invalid argument.");
        //    }
        //    catch (FormatException)
        //    {
        //        Console.WriteLine("Invalid argument.");
        //    }
        //    catch (OverflowException)
        //    {
        //        Console.WriteLine("Invalid argument, number too big.");
        //    }


        //    return result;
        //    }

        //static void EditName()
        //{
        //    if (!File.Exists(pathToSave))
        //    {
        //        Console.WriteLine("There is no file with history to edit.");
        //    }
        //    else
        //    {

        //    }
        //}

        //static void EditStart()
        //{
        //    if (!File.Exists(pathToSave))
        //    {
        //        Console.WriteLine("There is no file with history to edit.");
        //    }
        //    else
        //    {

        //    }
        //}

        //static void EditEnd()
        //{
        //    if (!File.Exists(pathToSave))
        //    {
        //        Console.WriteLine("There is no file with history to edit.");
        //    }
        //    else
        //    {

        //    }
        //}

        //static void EditDuration()
        //{
        //    if (!File.Exists(pathToSave))
        //    {
        //        Console.WriteLine("There is no file with history to edit.");
        //    }
        //    else
        //    {

        //    }
        //}
    }
}

