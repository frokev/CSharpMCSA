using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_exam_practice
{
    class ManageProgramFlow
    {
        static void Main(string[] args)
        {
            new MultithreadingExamples().ParallelInvoke();
        }
    }

    // ------------Implement multithreading and asynchronous processing----------------
    // Use the Task Parallel library, including theParallel.For method, PLINQ, Tasks; 
    // create continuation tasks; spawn threads by using ThreadPool; unblock the UI; 
    // use async and await keywords; manage data by using concurrent collections

    class MultithreadingExamples
    {
        // The Task Parallel library (System.Threading.Tasks)

        public void Task1() => Console.WriteLine("yo");
        public void Task2() => Console.WriteLine("hey");

        public void ParallelInvoke()
        {
            Parallel.Invoke(Task1, Task2); // Can execute multiple tasks/methods in parallel.
        }

        public void ParallelForEach()
        {
            var stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);

            // Returns ParallelLoopResult
            Parallel.ForEach(stack, item => Console.WriteLine(item)); // Iterate stack in parallel, we don't know if 1 or 2 is logged first
        }

        public void ParallelFor()
        {
            var nums = new int[] { 1, 2, 3 };
            var res = Parallel.For(0, nums.Length, (i, loopstate) => {
                Console.WriteLine(nums[i]); // Executed in parallel, we don't know if it logs 1, 2 or 3 first.
                loopstate.Stop(); // stop iteration
            });

            bool completed = res.IsCompleted; // Returns ParallelLoopResult so we can determine if it has successfully completed
        }

        public void PLinqTest()
        {
            var arr = new int[] { 1, 2, 3 };
            arr.AsParallel().Where(num => num == 2); // Run the LINQ query in parallel
            arr.AsParallel().AsOrdered().Where(num => num == 2); // Return with the order of elements in the queried array
            arr.AsParallel().AsOrdered().Where(num => num == 2).AsSequential(); // Convert ParallelQuery object into an IEnumerable

            var numbers = from number in arr.AsParallel().AsSequential() // Force sequential evaluation of the parallel query
                          where number == 2
                          select number;

        }

        public void ForAll()
        {
            var arr = new int[] { 1, 2, 3 };
            var res = arr.AsParallel().Where(num => num == 2);
            res.ForAll(num => Console.WriteLine(num)); // iterate a ParallelQuery object, to perform an action, runs in parallel
        }

        public void LinqExceptions()
        {
            var arr = new int[] { 1, 2, 3 };

            try
            {
                var res = arr.AsParallel().Where(n => n == 2);
            }
            catch (AggregateException e) // If an exception is thrown within a linq query an AggregateException is thrown
            {
                Console.WriteLine("catched exceptions: " + e.InnerExceptions.Count);
            }
        }

        public void CreateTasks()
        {
            // tasks run in the background (application will terminate even if the task isn't complete)



            var task = new Task(() => Console.WriteLine("hey")); // create
            task.Start(); // start (async)
            task.Wait(); // wait for completion

            var task2 = Task.Run(() => Console.WriteLine("yo")); // create and run
            task2.Start(); // run it again
            task2.Wait(); // wait for completion

            var withValue = Task.Run(() => 1);
            int value = withValue.Result; // wait for value and assign

            var tasks = new Task[3];
            tasks[0] = Task.Run(() => Console.WriteLine("yo"));
            Task.WaitAny(tasks); // wait for any task to complete
            Task.WaitAll(tasks); // wait for all tasks to complete

            withValue.ContinueWith(t => Console.WriteLine(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion); // Execute when task is complete, and successfull
            withValue.ContinueWith(t => Console.WriteLine("failed"), TaskContinuationOptions.OnlyOnFaulted); // Execute when task is complete, and unsuccessful

            var parent = Task.Factory.StartNew(() => // task with children
            {
                for (int i = 0; i < 3; i++)
                    Task.Factory.StartNew(() => Console.WriteLine("yo"), TaskCreationOptions.AttachedToParent);
            });

            parent.Wait(); // wait for all children to complete as well
        }

        public void AMethod(object i) => Console.WriteLine(i);

        public async void Threads() // We use async methods when we need to wait for tasks to complete.
        {
            // threads run in forground by default (app will not terminate until completion)

            Thread thread = new Thread(() => Console.WriteLine("hey"));
            thread.Start();
            thread.Join(); // wait until thread is complete/terminates. similar to task.wait

            var ps = new ParameterizedThreadStart(AMethod); // We can use this delegate to pass parameters to a thread
            var threadWithParams = new Thread(ps);
            threadWithParams.Start(99);

            var thread2 = new Thread((param) => Console.WriteLine(param)); // or we can use lambda to pass parameters
            thread2.Start(2);
            thread2.Abort(); // abort excecution, can leave resources and streams open

            ThreadPool.QueueUserWorkItem(stateInfo => { // add a method to the threadpool, runs in background
                Console.WriteLine("hey");
            });

            var task = Task.Run(() => 1);
            int result = await task; // async await is used to wait and get the result of a task.
        }

        public void ConcurrentCollections()
        {
            var data = new BlockingCollection<int>(2); // Can block threads from execution when data is full or empty
                                                       // Acts as a wrapper for ConcurrentQueue (first in first out) by default, we can specify another concurrent collection

            var data2 = new BlockingCollection<int>(new ConcurrentStack<int>(), 5); // Is now a wrapper for concurrent stack (last in first out)

            Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    data.Add(i); // the collection will block the calling thread when it reaches its size limit (tries to add a third element)
                }

                data.CompleteAdding(); // Tell data we are done adding objects
            });

            Task.Run(() =>
            {
                while (!data.IsCompleted) // While data is not empthy or we are done adding items, take an object
                    data.Take();
            });



            var que = new ConcurrentQueue<int>(); // Collections in the Collections.Concurrent namespace is threadsafe such that we can work on them in parallel
            for (int i = 0; i < 10; i++) que.Enqueue(i);
            Parallel.ForEach(que, (value) => {
                int res;
                while (!que.TryDequeue(out res) && !que.IsEmpty) ; // Guarantees that all elements will be dequeued in parallel (false if dequeue failed or queue empty)
            });

            var stack = new ConcurrentStack<int>(); // same as que except LIFO instead of FIFO
            stack.Push(1);
            int res2;
            stack.TryPop(out res2);

            var bag = new ConcurrentBag<int>(); // represents a threadsafe unordered collection

            var dic = new ConcurrentDictionary<string, int>(); // threadsafe dictionary
            dic.GetOrAdd("yo", (value) => 1);
            dic.TryAdd("yo", 2);
            dic.AddOrUpdate("yo", 4, (key, prevVal) => prevVal == 1 ? 4 : 2);
        }
    } // ---------------------------------------------------------------------------------


    // ----------------Manage multithreading--------------------
    // Synchronize resources; implement locking; cancel a long-running task; 
    // implement thread-safe methods to handle race conditions

    class ManageMultiThreading
    {
        public void SynchronizeResources()
        {
            // we can use parallel and tasks to synch resources
        }

        public void Locking()
        {
            object myLock = new object(); // our lock
            int x = 0;

            Task.Run(() =>
            {
                lock (myLock)
                {
                    // do stuff
                    x += 1;
                }
            });

            Task.Run(() =>
            {
                lock (myLock) // x will always end up as 4, without lock it could be 1, 3 or 4
                {
                    x += 3;
                }
            });
        }

        public void CancelThreads()
        {
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
        }

    } //-------------------------------------------------------------------

    // -------------------Implement program flow------------------------
    // Iterate across collection and array items; 
    // program decisions by using switch statements, if/then, and operators; 
    // evaluate expressions

    class ImplementProgramFlow
    {
        public void IterateCollections()
        {
            var list = new List<int>(); // arrays are iterated somewhat the same, iteration on items can be done in a nested loop

            foreach (int number in list)
            {
                Console.WriteLine(number);
            }

            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i]);
            }
            // can also use while, do-while
        }

        public void ProgramDecisions()
        {
            var obj = new { Name = "yo", State = "alabama" };
            string str = obj.Name;
            switch (str)
            {
                case "yo":
                    Console.WriteLine("Has name");
                    break;
                case "alabama":
                    Console.WriteLine("Has state");
                    break;
                default:
                    Console.WriteLine("not interested");
                    break;
            }

            if (str == obj.Name)
                Console.WriteLine("Has name");
            else if (str == obj.State)
                Console.WriteLine("Has state");
            else
                Console.WriteLine("not interested");
        }

        public void EvaluatingExpressions()
        {
            // && if first operand is false program will not evaluate the second (short-circuit operator)
            // & both will be evaluated every time

            // Monadic operators - one expression
            int i = 0;
            int j = i++; // return, then increment i (j is 0, i is 1)
            int l = ++i; // increment i, then return (l is 2)

            // Binary operators
            int k = (1 + 2) * 3; // order of execution is PEMDAS

            // Ternary operators
            int? r = (i == 2) ? 1 : 0; // Condition ? if-assignment : else-assignment

            // null-coalescing operator
            int n = r ?? 1; // return r if it's not null, else return 1.

            // null conditional operators
            int[] arr = null;
            int? c = arr?[2]; // returns null if arr is null, else element at index 2 (normally throws an exception)...
        }
    } // ---------------------------------------------------------------------

    // -------------Create and implement events and callbacks----------------
    // Create event handlers; subscribe to and unsubscribe from events; 
    // use built-in delegate types to create events; create delegates; 
    // lambda expressions; anonymous methods

    class EventsAndCallbacks
    {
        class MyEventArgs : EventArgs
        {
            public string Location { get; set; }

            public MyEventArgs(string location)
            {
                Location = location;
            }
        }

        class EventHandlers
        {
            public string Something { get; set; }
            public Action OnEventRaised { get; set; }

            public event Action OnEventRaised2 = delegate { };

            public event EventHandler<MyEventArgs> OnEventRaised3 = delegate { }; // We can use EventHandler class when listeners might need data from the caller object

            public void MyEvent()
            {
                OnEventRaised?.Invoke(); // Witout event construction we make sure it's not null before invoking
                OnEventRaised2(); // Using the event construction, we don't have to do null checks (it requires a delegate)
                OnEventRaised3(this, new MyEventArgs("location")); // takes this instance and an EventArgs (which we can use to pass data to listeners)
            }
        }

        public void EventHandlersTest()
        {
            var eh = new EventHandlers();
            Action listener = () => Console.WriteLine("Event raised");
            EventHandler<MyEventArgs> listener2 = (sender, args) => Console.WriteLine("Event raised", sender, args.Location);

            eh.OnEventRaised += listener; // bind a listener for MyEvent (subscribe)
            eh.OnEventRaised3 += listener2;
            eh.MyEvent(); // "Event raised" is printed
            eh.OnEventRaised -= listener; // unbind a listener for MyEvent (unsubscribe)

            // An anonymous (no reference variable existent) method in a task:
            Task.Run(() => Console.WriteLine("hey"));
        }
    } // -------------------------------------------------

    // --------------------Implement exception handling----------------------------
    // Handle exception types, including SQL exceptions, network exceptions, communication exceptions, network timeout exceptions; 
    // use catch statements; use base class of an exception; implement try-catchfinally blocks; throw exceptions; 
    // rethrow an exception; create custom exceptions; handle inner exceptions; handle aggregate exception


    // ---Handle exception types---

    // InvalidOperationException - Throwed when failure to invoke a method is caused by other reasons than invalid arguments.

    // ArgumentException - Bad arguments passed to a method
    // ArgumentNullException - An argument of a method that shouldn't be null is null
    // ArgumentOutOfRangeException - when the value of an argument is outside the allowable range of values e.g. 20 when it shouldn't be more than 10.

    // NullReferenceException - when there is an attempt to dereference a null object reference.
    // IndexOutOfRangeException - when an attempt is made to access an element of an array or collection with an index that is outside its bounds.
    // AccessViolationException - when there is an attempt to read or write protected memory

    // StackOverflowException - when the execution stack overflows because it contains too many nested method calls

    // OutOfMemoryException - when there is not enough memory to continue the execution of a program.

    // COMException - when an unrecognized HRESULT is returned from a COM method call

    // IOException - Thrown during input/output operations
    // SqlException - Invalid sql query, provides SqlError items (describe error)
    // LINQ queries are evaluated when results are requested

    // CommunicationsException - during WCF (Windows communication foundation) operations
    // TimeOutException - A network operation took too long


    // Use base class of an exception

    class Exceptions
    {
        public void UseBaseException()
        {
            try
            {
                int i = int.Parse("0");
                int j = 2 / i; // DivideByZeroException
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine("Can't divide by zero");
            }
            catch (Exception ex)
            {
                Console.WriteLine("something unexpected occured: ", ex.Message);
            }
            finally
            {
                // here we can clean up resources in case our program throwed an exception
            }
        }

        public void Rethrowing ()
        {
            try
            {
                try
                {
                    int i = int.Parse("0");
                    int j = j / i;
                }
                catch (Exception ex)
                {
                    throw new Exception("Inner exception", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Read inner exception: ", ex.InnerException.Message);
                throw ex; // rethrow
            }
        }

        public void AggregateExceptions()
        {
            try
            {
                // do something
            }
            catch (AggregateException ag)
            {
                foreach (Exception ex in ag.InnerExceptions) // Handling aggregateExceptions
                {
                    Console.WriteLine("Exception: ", ex.Message);
                }
            }
        }
    }
}
