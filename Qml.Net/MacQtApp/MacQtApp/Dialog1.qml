import QtQuick 2.2
import QtQuick.Dialogs 1.1

MessageDialog {
    id: messageDialog1
    title: "Overwrite?"
    icon: StandardIcon.Question
    text: "file.txt already exists.  Replace?"
    detailedText: "To replace a file means that its existing contents will be lost. " +
        "The file that you are copying now will be copied over it instead."
    standardButtons: StandardButton.Yes | StandardButton.YesToAll |
        StandardButton.No | StandardButton.NoToAll | StandardButton.Abort
    //Component.onCompleted: visible = true
    onYes: console.log("copied")
    onNo: console.log("didn't copy")
    onRejected: {
        console.log("aborted")
        Qt.quit()
    }
}
