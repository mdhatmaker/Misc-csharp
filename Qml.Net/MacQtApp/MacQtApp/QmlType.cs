//QmlType.cs
using Qml.Net;
using System.Threading.Tasks;

namespace MacQtApp
{
    [Signal("customSignal", NetVariantType.String)] // You can define signals that Qml can listen to.
    public class QmlType
    {
        /// <summary>
        /// Properties are exposed to Qml.
        /// </summary>
        [NotifySignal("stringPropertyChanged")] // For Qml binding/MVVM.
        public string StringProperty { get; set; }

        /// <summary>
        /// Methods can return .NET types.
        /// The returned type can be invoked from Qml (properties/methods/events/etc).
        /// </summary>
        /// <returns></returns>
        public QmlType CreateNetObject()
        {
            return new QmlType();
        }

        /// <summary>
        /// Qml can pass .NET types to .NET methods.
        /// </summary>
        /// <param name="parameter"></param>
        public void TestMethod(QmlType parameter)
        {
        }

        /// <summary>
        /// Async methods can be invoked with continuations happening on Qt's main thread.
        /// </summary>
        public async Task<string> TestAsync()
        {
            // On the UI thread
            await Task.Run(() =>
            {
                // On the background thread
            });
            // On the UI thread
            return "async result!";
        }

        /// <summary>
        /// Qml can also pass Qml/C++ objects that can be invoked from .NET
        /// </summary>
        /// <param name="qObject"></param>
        public void TestMethodWithQObject(dynamic o)
        {
            string result = o.propertyDefinedInCpp;
            o.methodDefinedInCpp(result);

            // You can also listen to signals on QObjects.
            var qObject = o as INetQObject;
            var handler = qObject.AttachSignal("signalName", parameters => {
                // parameters is a list of arguements passed to the signal.
            });
            handler.Dispose(); // When you are done listening to signal.

            // You can also listen to when a property changes (notify signal).
            handler = qObject.AttachNotifySignal("property", parameters => {
                // parameters is a list of arguements passed to the signal.
            });
            handler.Dispose(); // When you are done listening to signal.
        }

        /// <summary>
        /// .NET can activate signals to send notifications to Qml.
        /// </summary>
        public void ActivateCustomSignal(string message)
        {
            this.ActivateSignal("customSignal", message);
        }
    }
}