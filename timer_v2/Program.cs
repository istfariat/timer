

namespace TimerApp
{
    internal class Program
    {

        static void Main(string[] args)
        {
            System.Console.WriteLine("This is timer, type 'start' to start a timer, type 'stop' to stop it. You can edit name of running timer by typing 'edit'.");

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            int i = 0;

            while (i < 10)
            {
                userInput(timer);
                i++;
            }

            Console.ReadLine();
        }

        static void userInput(System.Diagnostics.Stopwatch clock)
        {
            string command = System.Console.ReadLine();
            switch (command)
            {
                case "start":
                    startTimer(clock);
                    break;
                case "stop":
                    stopTimer(clock);
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    userInput(clock);
                    break;
            }
            return;
        }
            
        static void startTimer (System.Diagnostics.Stopwatch timer)
        {
            if (timer.IsRunning)
            {                
                Console.WriteLine("Timer already running, it is {0} currently", RoundToSeconds(timer.Elapsed));
            }
            else
            {
                timer.Start();
            }
        }

        static void stopTimer(System.Diagnostics.Stopwatch timer)
        {
            if (timer.IsRunning) 
            { 
                timer.Stop();
                System.Console.WriteLine("Last entry was {0} long", RoundToSeconds(timer.Elapsed));
                timer.Reset();
            }
            else
            {
                Console.WriteLine("There is no timer runnig to stop");
            }
            
        }

        static TimeSpan RoundToSeconds (TimeSpan timeInput)
        {
            int precision = 0; // how many digits past the decimal point
            const int TIMESPAN_SIZE = 7; // it always has seven digits
                                         // convert the digitsToShow into a rounding/truncating mask
            int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));

            TimeSpan roundedTimeSpan = new TimeSpan(((long)Math.Round((1.0 * timeInput.Ticks / factor)) * factor));
            return roundedTimeSpan;
        }

        static string editName (string name)
        {
            Console.WriteLine("Enter new name for a timer: ");            
            string newName = Console.ReadLine();
            return newName;
        }

    }
}
