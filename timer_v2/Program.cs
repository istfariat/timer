
using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

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
        
        static void Main(string[] args)
        {
                        
            //while (true)
            //{
            //    Console.WriteLine("Active window:\t {0}", GetActiveWindowTitle());
            //    Thread.Sleep(2000);
            //}



                    Stopwatch timer = new Stopwatch();
            int i = 0;

            Console.WriteLine("This is timer, type 'start' to start a timer, type 'stop' to stop it. You can edit name of running timer by typing 'name 1', 'name 2', 'name 3'.");

            while (i < 100)
            {
                //Console.WriteLine("Names of timer:\nname1: {0}\nname2: {1}\nname3: {2}", timerName[0], timerName[1], timerName[2]);
                userInput(timer);
                i++;
            }

            Console.ReadLine();
        }

        static void userInput(Stopwatch clock)
        {
            Console.Write("> ");
            string command = Console.ReadLine();
            switch (command)
            {
                case "start":
                    startTimer(clock);
                    break;
                case "stop":
                    stopTimer(clock);
                    break;
                case "name 1" or "name 2" or "name 3":
                    if (clock.IsRunning)
                        editName(command);
                    else
                        Console.WriteLine("No timer running to edit.");
                    break;
                case "show":
                    Console.WriteLine("Here is the history:");
                    for (int i = 0; i < history.Count; i++)
                    {
                        Console.WriteLine("Timer entry {0}:", i + 1);
                        for (int j = 0; j < 6; j++)
                            Console.WriteLine("\t {0}{1}", fields[j], history[i][j]);
                    }
                    break;
                case "save":
                    SaveFile();
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    userInput(clock);
                    break;
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


                history.Add(newEntry);
                timer.Start();
                history[history.Count - 1][0] = DateTime.Now.ToString();
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

        static void editName(string userInput)
        {

            string[] workStep = new string[3] { "field", "object", "task/stage" };
            int workIndex = int.Parse(userInput.Substring(5)) - 1;

            Console.WriteLine("Enter name of your {0} of work: ", workStep[workIndex]);
            history[history.Count - 1][workIndex + 3] = Console.ReadLine();

            return;
        }

        static void SaveFile ()
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

            
            
        }

        static void LoadFile()
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
    }
 }
