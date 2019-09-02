using System;
using Qml.Net;
using Qml.Net.Runtimes;

namespace MacQtApp
{
    class Program
    {
        /*static int Main(string[] args)
        {
            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    // TODO: Register your .NET types.
                    // Qml.RegisterType<NetObject>("test");

                    engine.Load("Main.qml");

                    return app.Exec();
                }
            }
        }*/
        static int Main(string[] args)
        {
            RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();

            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    // TODO: Register your .NET types to be used in Qml
                    Qml.Net.Qml.RegisterType<QmlType>("TestApp", 1, 1);   // in QML file: import TestApp 1.1
                    Qml.Net.Qml.RegisterType<YourNetObject>("YourApp", 2, 1);   // in QML file: import YourApp 2.1

                    
                    var netObject = new YourNetObject();
                    // You can attach signal handlers from both .NET and QML:
                    netObject.AttachToSignal("signalName", new Action<string, int>((param1, param2) =>
                    {
                        Console.WriteLine("Signal raised from .NET! {0} {1}", param1, param2);
                    }));


                    engine.Load("Main.qml");


                    // You can raise signals from .NET:
                    netObject.ActivateSignal("signalName", "param", 3);


                    return app.Exec();
                }
            }
        }
    }
}
