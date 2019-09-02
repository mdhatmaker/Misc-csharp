//Main.qml
import QtQuick 2.7
import QtQuick.Controls 2.0
import QtQuick.Layouts 1.0
import QtQuick.Dialogs 1.1
import TestApp 1.1
import YourApp 2.1

ApplicationWindow {
    id: win1
    visible: true
    width: 640
    height: 480
    title: qsTr("Hello World")
    
    Dialog1 { id: messageDialog1; visible: false; }
    Dialog2 { id: messageDialog2; visible: false; }

    Rectangle {
        id: page
        //width: 320; height: 480
        anchors.fill: parent
        color: "lightgray"
        
        property alias button: button1

        Button {
            anchors.centerIn: parent
            id: button1
            text: qsTr("Press Me")
            font.pointSize: 18; font.bold: false
            //onClicked: messageDialog2.show(qsTr("Button pressed"))
            onClicked: messageDialog1.visible = true
        }
        //button.onClicked: messageDialog.show(qsTr("Button pressed"))

        Text {
            id: helloText
            text: "Hello world!"
            y: 30
            anchors.horizontalCenter: page.horizontalCenter
            font.pointSize: 24; font.bold: true
        }

        Grid {
            id: colorPicker
            x: 4; anchors.bottom: page.bottom; anchors.bottomMargin: 4
            rows: 2; columns: 3; spacing: 3

            Cell { cellColor: "red"; onClicked: helloText.color = cellColor }
            Cell { cellColor: "green"; onClicked: helloText.color = cellColor }
            Cell { cellColor: "blue"; onClicked: helloText.color = cellColor }
            Cell { cellColor: "yellow"; onClicked: helloText.color = cellColor }
            Cell { cellColor: "steelblue"; onClicked: helloText.color = cellColor }
            Cell { cellColor: "black"; onClicked: helloText.color = cellColor }
        }
    }

    
    YourNetObject {
        id: o
        // You can attach signal handlers from both .NET and QML:
        onSignalName: function(param1, param2) {
            console.log("Signal raised from QML!")
        }
        Component.onCompleted: {
            // Outputs "hello!"
            console.log(o.Prop)
            console.log(o.PropI)
            // Outputs "2"
            console.log("YourNetObject result = " + o.add(1, 1))
        }

    }


    QmlType {
        id: test
        Component.onCompleted: function() {
            // We can read/set properties
            console.log(test.stringProperty)
            test.stringPropertyChanged.connect(function() {
                console.log("The property was changed!")
            })
            test.stringProperty = "New value!"

            // We can return .NET types (even ones not registered with Qml)
            var netObject = test.createNetObject();

            // All properties/methods/signals can be invoked on "netObject"
            // We can also pass the .NET object back to .NET
            netObject.testMethod(netObject)

            // We can invoke async tasks that have continuation on the UI thread
            var task = netObject.testAsync()
            // And we can await the task
            Net.await(task, function(result) {
                // With the result!
                console.log(result)
            })

            // We can trigger signals from .NET
            test.customSignal.connect(function(message) {
                console.log("message: " + message)
            })
            test.activateCustomSignal("test message!")
        }
        function testHandler(message) {
            console.log("Message - " + message)
        }
    }

}