using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace csharp_exam_practice
{   
    // Create and Use Types
    class CreateUseTypes
    { 
        static void Main(string[] args)
        {
            new ConsumeTypes();
            Console.ReadKey();
        }
    }

    class CreateTypes
    {
        public CreateTypes()
        {
            string tostr = new OverrideMethods().ToString();

            Console.WriteLine(tostr);
        }

        // Value types - Structs, Enumerations
        // Value type - value is copied, Reference type - reference is copied.
        // All value types derived from System.ValueType
        // Each value has a default type, contains value of the type, cannot be null (unless nullable type), 
        // cannot derive new type from value type. Reference types and structs can have interface.

        // Structs are used to encapsulate groups of related variables

        // can implement interfaces, cannot inherit from another struct, can contain several members (constructor, methods, events, etc..)
        public void Structs()
        {
            // unassigned values, will be default
            Coords coords = new Coords();

            // default constructor
            Coords coords1;
            coords1.x = 5;
            coords1.y = 8;

            // custom constructor
            Coords coords2 = new Coords(1, 2);

            Console.Write("unassigned: x = {0}, y = {1}\n", coords.x, coords.y);
            Console.Write("assigned, no parameters: x = {0}, y = {1}\n", coords1.x, coords1.y);
            Console.Write("assigned w/ parameters: x = {0}, y = {1}\n", coords2.x, coords2.y);
        }

        // Simple struct
        // If includes constructor w/ parameters, all members must be assigned. If default constructor, default values will be assigned.
        public struct Coords
        {
            public int x, y;

            public Coords(int p1, int p2)
            {
                x = p1;
                y = p2;
            }
        }

        // Enum is a set of named constants. Can be nested in class or struct. Can be any integral type except char.
        // Can define type like: "enum Day : byte {}"
        public enum Day { Sat = 1, Sun, Mon, Tue, Wed, Thur, Fri }; // Usage "Day.Sat" returns 1.

        // create reference types, generic types, constructors, static variables, methods, classes, extension methods.

        // Reference types store references to their data (objects), value types directly contain their data
        // The following are reference type.
        class ReferenceType
        {
            interface AlsoReferenceType { }
            delegate void ReferenceTypeDelegate();

            // built in reference types
            dynamic rtdyn;
            string rtstr;
            object obj;
        }

        // Generic Types
        public void MethodWGenericType<T>()
        {
            List<T> arr = new List<T>();
        }

        // A static member belongs to the type, rather than a specific object. Must be referenced trough type, not instance
        public static void StaticMethod() { Console.WriteLine("I'm static"); }

        // Named parameters makes for freedom in ordering parameters, and improves readability
        public void NamedParameters(int x, int y, int z)
        {
            if ("true" is string)
            {
                return;
            }
            
            NamedParameters(z: 1, y: 2, x: 3);
        }

        // Optional parameters. Can be specified by giving the parameter a default value
        // Optional parameters must be after required params.
        public void OptionalParams(string firstName, int age = 0)
        {
            Console.WriteLine("Name: {0}, Age: {1}", firstName, age);
        }

        // Indexed properties
        public class IndexedProps<T>
        {
            string hello = "hey";
            int integer = 2;
            bool abool = true;

            public T this[int i]
            {
                get
                {
                    switch(i)
                    {
                        case 0:
                            if (typeof(T) != typeof(string)) break;
                            return (T)(object)hello;
                        case 1:
                            if (typeof(T) != typeof(int)) break;
                            return (T)(object)integer;
                        case 2:
                            if (typeof(T) != typeof(string)) break;
                            return (T)(object)abool;
                    }
                    return default(T);
                }
            }
        }

        // Override methods
        class OverrideMethods
        {
            public override string ToString()
            {
                return base.ToString() + " Overridden";
            }
        }

        // Overloaded methods
        public int Overloaded(int i)
        {
            return i;
        }

        public string Overloaded(string str)
        {
            return str;
        }

        public T Overloaded<T>(T obj)
        {
            return obj;
        }

    }

    // Extension methods allows static methods to be called on instance method syntax. 
    // Can extend an existing type. Must take a parameter that reprecents the instance the method is to operate on.
    // Must import namespace defining sponsor, must be top level class
    // Class and method must be static
    public static class StringWrapper
    {
        public static string ToLeetSpeak(this string str)
        {
            str = str.Replace('e', '3').Replace('E', '3');
            return str;
        }
    }

    class ConsumeTypes
    {
        public ConsumeTypes()
        {
            BoxUnbox();
        }

        // Box or Unbox to convert between value types
        // Boxing is converting a value to the type object, unboxing extracts the value from the object
        public void BoxUnbox()
        {
            int i = 3;
            string s = "yolo";
            bool b = false;

            // Boxing
            List<object> li = new List<object>();
            li.Add(i);
            li.Add(s);
            li.Add(b);

            // Unboxing
            int ui = (int)li[0];
            string us = (string)li[1];
            bool ub = (bool)li[2];

            Console.WriteLine("ui: {0}, us: {1}, ub: {2}", ui, us, ub);
        }

        // Casting is required when data loss might occur (Called explicit conversions).
        public int Casting()
        {
            double x = 1234.7;
            int a;
            a = (int)x;
            return a;
        }

        // Implicit conversions, no casting required. For smaller to larger integral types, conversion of base-class instance to derived class
        // There is also user-defined conversions, you can make with special methods
        // There are helper classes for doing conversions (System.Convert etc..)
        public long ImplicitConversion()
        {
            int num = 2147483647;
            long bigNum = num;
            return bigNum;
        }

        // Handle dynamic types
        // Dynamic types are handled at runtime, type conflicts or invalid references will throw an exception at runtime
        public void DynamicTypes(dynamic obj)
        {
            // obj can be anything! string, int, float.
            Console.WriteLine(obj);
        }

        // TODO: interoperability w/ code that access DOM APIs
    }

    // Enforce encapsulation
    class EnforceEncapsulation
    {
        public EnforceEncapsulation()
        {

            // Encapsulation by explicit interface implementation
            Calculations calc = new Calculations();
            // calc.Calculate(1, 2); Not valid. will result in error
            IAddition add = calc;
            IMultiplication multiply = calc;
            add.Calculate(1, 2);
            multiply.Calculate(1, 2);
            
        }

        // Encapsulation by using properties
        public string Name { get; private set; }
        // Encapsulation by using accessors, incl public, private, protected, internal (access modifiers for type and type members)
        // public: Access not restricted
        // private: Limited to containing type
        // protected: Limited to containing class, and types derived from class
        // internal: Limited to current assembly (chunk of precompiled code, like an .exe or a .dll file)
        private string lastName;
        public string GetLastName() => lastName + 1;
        public string SetLastName(string lastName) => this.lastName = lastName;

        // Encapsulation by explicit interface implementation
        // If two interface members (w/ same name) does not perform the same function, 
        // this can lead to incorrect implementation 
        // We can fix it with explicit interface implementation
        interface IAddition { int Calculate(int x, int y); }
        interface IMultiplication { int Calculate(int x, int y); }
        class Calculations : IAddition, IMultiplication
        {
            int IAddition.Calculate(int x, int y) => x + y;
            int IMultiplication.Calculate(int x, int y) => x * y;
        }
    }

    // Create and implement a class hierarchy
    class CreateImplementHierarchy
    {
        public CreateImplementHierarchy()
        {
            WNumber num = new WNumber(1);
            num.CompareTo(new WNumber(2));
        }

        // Design and implement an interface
        interface IMaths
        {
            int Add(int x, int y);
            int Multiply(int x, int y);
            int Subtract(int x, int y);
        }
        // Implementing interface
        class Maths : IMaths
        {
            // virtual keyword allows a method or property to be overridden in a derived class
            // members marked with abstract keyword must be implemented in a derived class. 
            // abstract modifier indicates incomplete implementation
            public virtual int Add(int x, int y) => x + y;
            public int Multiply(int x, int y) => x * y;
            public int Subtract(int x, int y) => x - y;
        }

        // Inherit from base class
        class DerivedClass : Maths
        {
            public override int Add(int x, int y)
            {
                // inherit member from base class
                // base keyword cannot be used within static methods
                return base.Add(x, y) + 1;
            }
        }

        // Create and implement classes based on the IComparable, IEnumerable, IDisposable, and IUnknown interfaces

        // IComparable - For ordering or sorting its instances
        class WNumber : IComparable<WNumber>
        {
            private int number;
            public WNumber(int number) => this.number = number;
            public int CompareTo(WNumber other) => number.CompareTo(other);
        }

        // IEnumerator - simple iteration over non-generic collection (can use foreach on WString)
        class WString : IEnumerable
        {
            private string[] strList = new string[] { "list", "of", "strings" };

            public IEnumerator GetEnumerator()
            {
                return new WStringEnumerator(strList);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new WStringEnumerator(strList);
            }
        }

        class WStringEnumerator : IEnumerator
        {
            private string[] values;
            private int index = -1;

            public WStringEnumerator(string[] values) => this.values = values;

            public object Current => values[index];

            public bool MoveNext()
            {
                index++;
                return index < values.Length;
            }

            public void Reset()
            {
                index = 0;
            }
        }

        // IDisposable - A mechanism for releasing unmanaged resources.
        class MyResource : IDisposable
        {
            private IntPtr handle; // External unmanaged resource
            // ManagedResource other;
            private bool disposed = false;

            public MyResource(IntPtr handle)
            {
                this.handle = handle;
            }

            // So parent can dispose manually
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposable)
            {
                // check if already disposed
                if (this.disposed) return;

                //If disposable, dispose managed resources. else the runtime has called it
                if (disposable)
                {
                    // this.other.Dispose();
                }

                // Dispose unmanaged resources, Call cleanup methods
                // CloseHandle(handle);
                handle = IntPtr.Zero;

                disposed = true; // We are done disposing
            }

            // So finalizer can dispose unmanaged resources at runtime
            ~MyResource()
            {
                // No need to dispose managed resources, only unmanaged
                Dispose(false);
            }
        }

        // IUnknown
    }
}
