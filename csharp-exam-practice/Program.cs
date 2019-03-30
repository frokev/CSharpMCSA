using System;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_exam_practice
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    new MultiThreading();
        //    Console.ReadKey();
        //}
    }

    // testing short-circuit operator
    class ShortCircuit {

        public ShortCircuit()
        {
            // if first operator is true, the second is not evaluated
            bool res = true || IsTrue();
            // if first operator is false, the second is not evaluated
            bool res2 = false && IsTrue();
            Console.WriteLine(res);
            Console.WriteLine(res2);

            // we can evaluate all operators by using the a single character "|" or "&" (Now we see the print in console)
            bool res3 = false & IsTrue();
            Console.WriteLine(res3);
        }

        private bool IsTrue()
        {
            bool res = false;
            Console.WriteLine("isTrue called");
            return res;
        }
    }

    // testing delegates (delegates is useful when you need to point to one or multiple functions)
    class Delegates
    {
        public delegate int Calculation(int x, int y);
        public Delegates()
        {
            int x = 3;
            int y = 5;
            Calculation multiply = Multiply;
            Calculation add = Addition;
            PrintCalc(x, y, multiply);
            PrintCalc(x, y, add);

            // Delegete can hold multiple functions (Multicast delegate)
            Calculation calc = Multiply;
            calc += Addition;
            PrintCalc(x, y, calc);

            Console.WriteLine("\nCallbacks: ");
            UseCallback(PrintCallback);
        }

        // Delegetes are best taken advantage of as a callback.
        public delegate void Callback(int x);
        public void UseCallback(Callback callback)
        {
            for (int i = 0; i < 3; i++)
            {
                callback(i);
            }
        }

        public void PrintCallback(int i)
        {
            Console.WriteLine(i);
        }

        public void PrintCalc(int x, int y, Calculation calculation)
        {
            Array functions = calculation.GetInvocationList();
            if (functions.Length < 2)
            {
                int res = calculation(x, y);
                Console.WriteLine("calculation is: " + res);

            } else
            {
                Console.WriteLine("\nUsing multicasting:");
                foreach (Calculation func in functions) {
                    int res = func(x, y);
                    Console.WriteLine("calculation is: " + res);
                }
            }
     
        }

        public int Multiply(int x, int y)
        {
            return x * y;
        }

        public int Addition(int x, int y)
        {
            return x + y;
        }
    }

    class Lambdas
    {
        public Lambdas()
        {
            // Encapsulation (delegates)
            Func<int, int> square = x => x * x;
            Console.WriteLine(square(5));

            Func<int, int, int> addAndWrite = (x, y) =>
            {
                int res = x + y;
                Console.WriteLine(res);
                return res;
            };

            addAndWrite(3, 5);

            Action line = () => { Console.WriteLine("hello"); };
            line();
            //----------------------------

            // Local function using lambda
            void line2() => Console.WriteLine("world");
            line2();
        }
    }

    // Testing / triggering events
    class Events
    {
        public event Action OnChange;

        public Events()
        {
            OnChange += PrintSomething;
            OnChange();
            var ms = new MailService();
            ms.MailSent += ms_MailSent;

        }

        // Naming a subscriber function with the instance name it is used with (instanceName_EventHandlerName). Like it's done in the docs.
        private void ms_MailSent(object sender, MailService.OnMailSentEventArgs e)
        {
            Console.WriteLine(e);
        }

        public void PrintSomething()
        {
            Console.WriteLine("Hello world");
        }

        public class MailService
        {
            public event EventHandler<OnMailSentEventArgs> MailSent;

            public void SendMail()
            {
                string mail = "Mail: Hello World!";
                Console.WriteLine("Sending mail");
                Thread.Sleep(3000);
                OnMailSentEventArgs args = new OnMailSentEventArgs();
                args.Mail = mail;
                OnMailSent(args);
                Console.WriteLine("Mail sent");
            }

            protected virtual void OnMailSent(OnMailSentEventArgs e)
            {
                EventHandler<OnMailSentEventArgs> handler = MailSent;
                handler?.Invoke(this, e);
            }

            public class OnMailSentEventArgs : EventArgs
            {
                public string Mail;
            }
        }
    }

    // Share processing. Threads are components of a process, where code is executed concurrently with the process
    class MultiThreading
    {
        public MultiThreading()
        {
            Thread thread = new Thread(new ThreadStart(ThreadMethod)); // or ParameterizedThreadStart with thread.start(args) to pass parameters
            Thread bgThread = new Thread(new ThreadStart(BackgroundThread));
            NormalMethod();
            thread.Start(); // Process will not terminate until finished
            Console.WriteLine("Runs before other thread finishes");
            thread.Join(); // Wait for thread to finish
            Console.WriteLine("Runs after other thread finished");

            bgThread.Start();
            bgThread.IsBackground = true; // Does not prevent process from terminating (terminates with process).

            //We can cancel threads
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            //cts.Cancel(); -- we can cancel like this

            //Tasks run in ThreadPool
            Task<string> t = Task.Run(async () => {
                await Task.Delay(1000);
                Console.WriteLine("\nI'm a task");
                await Task.Delay(3000);
                return "task finished";
            }, token);

            t.ContinueWith((task) =>
            {
                Console.WriteLine(task.Result);
            });

            t.Wait(); // Wait for task to complete

            Task[] tasks = new Task[1];
            tasks[0] = Task.Run(() => Console.WriteLine("\nA task in array"));
            Task.WaitAll(tasks); // We can wait for all tasks in one command

            // We can make sure no one else has locked our object and lock it ourselves while we make changes by using 'lock'.
            // Without lock we can read the object even if another thread has a lock on it.
            string str = "heyo";
            Task.Run(() =>
            {
                lock (str) // Deadlocks can happen when we nest lock statements. Such that both threads are waiting for eachother endlessly.
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("\nheyo finished");
                }
            });

            Thread.Sleep(500);
            lock (str)
            {
                Console.WriteLine(str); // Must wait for our task to finish
            }

            Thread.Sleep(5000);
            //Environment.Exit(0);
        }

        public void ThreadMethod()
        {
            Thread.Sleep(3000);
            Console.WriteLine("\nThis is run on a separate thread");
            Thread.Sleep(3000);
        }

        public void NormalMethod()
        {
            Console.WriteLine("This is run on main thread");
        }

        public void BackgroundThread()
        {
            Thread.Sleep(6000);
            Console.WriteLine("\nThis will not run when the process has finished");
            Thread.Sleep(60000000);
            Console.WriteLine("I will probably never show");
        }
    }

    class ParallelLoopTest
    {
        public ParallelLoopTest() {

            Parallel.For(0, 10, (i) => // Iterations run in parallel / concurrently
            {
                Thread.Sleep(2000);
                Console.WriteLine(i);
            });
        }
    }
}
