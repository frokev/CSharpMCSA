using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace csharp_exam_practice
{   
    // Create and Use Types
    class CreateUseTypes
    { 
        //static void Main(string[] args)
        //{
        //    ManipulateStrings.FormatStrings();
        //    Console.ReadKey();
        //}
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
            // Managed resource (whatever the GC can clean up).
            // Implements IDisposable which means we can manually dispose or leave it to finalizer.
            // Finalizer is slow so manual disposal is prefered. 
            // Lets pass on the option to manually dispose it by implementing the IDisposable.
            private Component component = new Component();

            // External unmanaged resource,
            // we should offer the user a way to dispose of the resource.
            // Make sure it's taken care of with finalizer if user doesn't.
            // Log a massage to inform that unmanaged resources should be disposed manually (finalizer is slow at cleaning).
            private IntPtr handle; 

            private bool disposed = false;

            public MyResource(IntPtr handle)
            {
                this.handle = handle;
            }

            // So parent can dispose manually
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this); // We have already disposed of unmanaged resources, no need for the finalizer to do it again
            }

            protected virtual void Dispose(bool disposing)
            {
                // check if already disposed
                if (this.disposed) return;

                //If disposable, dispose managed resources. else the runtime has called it
                if (disposing)
                {
                    component.Dispose();
                }

                // Dispose unmanaged resources, Call cleanup methods
                CloseHandle(handle);
                handle = IntPtr.Zero;
                

                disposed = true; // We are done disposing
            }

            // Use interop to call the method necessary
            // to clean up the unmanaged resource.
            [System.Runtime.InteropServices.DllImport("Kernel32")]
            private extern static bool CloseHandle(IntPtr handle);

            // So finalizer can dispose unmanaged resources at runtime
            ~MyResource()
            {
                // No need to dispose managed resources, only unmanaged
                Dispose(false);
            }
        }

        // IUnknown? (very little documentation)
    }

    // Find, execute, and create types at runtime by using reflection

    // [assembly: CLSCompliant(true)]    Specify CLS compliance for entire assembly

    class FiExCrTyRuRe
    {
        // Create and apply attributes
        // Attributes - Extensible mechanism for adding custom information to code elements. C# and .NET includes predefined attributes
        [Obsolete]
        public class ObsoleteClass { }

        // The attribute maps to XML element named Customer
        // attribute tells XML serializer (in System.Xml.Serialization) how an object is represented in XML
        // [XmlElement("Customer", Namespace = "csharp_exam_practice")]
        public class CustomerEntity { }

        // Multiple attributes can be specified for a single code element
        // [Serializable, Obsolete, CLSCompliant(false)]

        // Caller info attributes
        // [CallerMemberName] applies the caller’s member name
        // [CallerFilePath] applies the path to caller’s source code file
        // [CallerLineNumber] applies the line number in caller’s source code file

        // Create attributes - by deriving from Attribute
        class Author : Attribute
        {
            private string name;
            public double version;

            public Author(string name)
            {
                this.name = name;
                version = 1.0;
            }
        }

        // Read attributes
        [Author("kevin", version = 2)]
        public void Method()
        {
            MethodAttributes attr = typeof(FiExCrTyRuRe).GetMethod("Author").Attributes;
        }

        // Generate code
        class GenerateCode
        {
            //public static string GenerateCSharpCode(CodeCompileUnit compileunit)
            //{
            //    // Generate the code with the C# code provider.
            //    CSharpCodeProvider provider = new CSharpCodeProvider();

            //    // Build the output file name.
            //    string sourceFile;
            //    if (provider.FileExtension[0] == '.')
            //    {
            //        sourceFile = "HelloWorld" + provider.FileExtension;
            //    }
            //    else
            //    {
            //        sourceFile = "HelloWorld." + provider.FileExtension;
            //    }

            //    // Create a TextWriter to a StreamWriter to the output file.
            //    using (StreamWriter sw = new StreamWriter(sourceFile, false))
            //    {
            //        IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");

            //        // Generate source code using the code provider.
            //        provider.GenerateCodeFromCompileUnit(compileunit, tw,
            //            new CodeGeneratorOptions());

            //        // Close the output file.
            //        tw.Close();
            //    }

            //    return sourceFile;
            //}
        }

        // Use types from the System.Reflection namespace, including Assembly, PropertyInfo, MethodInfo, Type
        // Inspecting the metadata and compiled code of an assembly at runtime is called reflection.

        Type t = typeof(string);
        public void func()
        {
            Console.WriteLine(t.BaseType); // Object
            Console.WriteLine(t.Namespace); // System
            Console.WriteLine(t.Name); // SpecialFolder
            Console.WriteLine(t.FullName); // System.Environment+SpecialFolder

            foreach (Type iType in typeof(string).GetInterfaces())
                Console.WriteLine(iType.Name); // IEnumerable, ICloneable, IComparable, IConvertible, IEquatable
        }

        public class Example
        {
            private int fraction;
            public int AnInt { get; set; } = 2;
            public Example(int i) => fraction = i;
            public int Method(int i) => fraction * i;


            public void TestAssembly()
            {
                Assembly assem = typeof(Example).Assembly;
                Console.WriteLine(assem.GetName().CodeBase);
                object o = assem.CreateInstance("csharp_exam_practice.FiExCrTyRuRe+Example", false, BindingFlags.ExactBinding, null, new object[] { 2 }, null, null);
                MethodInfo m = o.GetType().GetMethod("Method");
                object ret = m.Invoke(o, new object[] { 2 });
                Console.WriteLine("Result of Method: {0}", ret);
                PropertyInfo pi = o.GetType().GetProperty("AnInt", typeof(int));
                Console.WriteLine("Property has name {0} and it's value is {1}", pi.Name, pi.GetValue(o));
            }
        }
    }

    // Manage the object life cycle - See line 368

    // Manipulate strings
    // Manipulate strings by using the StringBuilder, StringWriter, and StringReader classes; 
    // search strings; enumerate string methods; format strings; use string interpolation

    class ManipulateStrings : IDisposable
    {
        // Good for manipulating strings. Strings are immutable, StringBuilder is mutable. 
        // Each change to a String will create a new copy on the heap, and a new reference on the stack. 
        // StringBuilder will modify itself on the heap, while keeping the same reference.
        StringBuilder stringBuilder;
        StringWriter stringWriter = new StringWriter();
        StringReader stringReader;

        // Format strings
        public static void FormatStrings()
        {

            // Actual format strings
            // We can use those to convert types

            // hex to int
            int thousand = int.Parse("3e8", System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine("Hex (3e8) to int: " + thousand);

            // round to two decimals
            string numStr = string.Format("{0:F2}", 134.527);
            Console.WriteLine("134.527 rounded to two: " + numStr);

            // to hexadecimal
            string hexStr = string.Format("{0:X}", 47);
            Console.WriteLine("47 in hex: " + hexStr);
            
        }

        // Search strings
        public void Search(ref string str)
        {
            bool doHave = stringBuilder.ToString().Contains(str);
            Console.WriteLine("Is {0} found: {1}", str, doHave);
        }

        // String interpolation
        public void Interpolation(object obj)
        {
            string str = $"This is interpolated: {obj}";
            Console.WriteLine(str);
        }

        // Enumerate string
        public void Enumerate()
        {
            Console.WriteLine("Enumerating\n");
            foreach (char ch in stringBuilder.ToString())
            {
                Console.WriteLine("Char: {0}", ch);
            }
        }

        // StringReader enables us to read async, or not.
        // We can read characters, lines and the whole string
        public async void ReadStrings(StringReader sr)
        {
            stringReader = sr;
            char[] buff = new char[50];

            // Read character by character into a buffer (char array)
            int bytesRead = await stringReader.ReadAsync(buff, 0, 50);
            Console.WriteLine("Char buffer: " + buff);

            // line by line
            string currString = await stringReader.ReadLineAsync();
            while (currString != null)
            {
                Console.WriteLine($"Next line: {currString}");
            }

            // Whole string
            string theString = await stringReader.ReadToEndAsync();
            Console.WriteLine("Whole string: {0}", theString);
        }

        // StringWriter (write stuff to a string or stream) StringBuilder cannot write to a stream

        public void WriteStr()
        {
            stringWriter.Write("Hello World");
            Console.WriteLine("StringWriter wrote: " + stringWriter);
        }

        // Dispose managed resources
        public void Dispose()
        {
            stringReader.Dispose();
            stringWriter.Dispose();
        }
    }

}
