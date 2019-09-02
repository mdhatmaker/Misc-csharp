using System;
using Qml.Net;

namespace MacQtApp
{
    // This class will be registered so it can be used within QML files.

    // You can raise signals from .NET:
    // netObject.ActivateSignal("signalName", "param", 3);

    /* // You can attach signal handlers from both.NET and QML:
    app.AttachToSignal("signalName", new Action<string, int>((param1, param2) =>
    {
        Console.WriteLine("Signal raised!");
    }));
    */

    // Your .NET objects can define signals (with or without parameters).
    //[Signal("signalName")]
    [Signal("signalName", NetVariantType.String, NetVariantType.Int)]
    public class YourNetObject
    {
        public string Prop { get; set; }
        public int PropI { get; set; }

        public YourNetObject()
        {
            this.Prop = "hello!";
            PropI = 5;
        }

        public int Add(int value1, int value2)
        {
            return value1 + value2;
        }
    }
}
