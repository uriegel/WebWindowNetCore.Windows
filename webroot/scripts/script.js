const source = new EventSource("http://localhost:20000/sse/test")
source.onmessage = (event) => console.log("SSE event", event.data)

const btn1 = document.getElementById("button")
const btnMinimize = document.getElementById("buttonMinimize")
const btnRestore = document.getElementById("buttonRestore")
const btnMaximize = document.getElementById("buttonMaximize")
const btnClose = document.getElementById("buttonClose")
const btnWindowState = document.getElementById("buttonWindowState")
const btnDevTools = document.getElementById("buttonDevTools")
const dropZone = document.getElementById("dropZone")

dropZone.addEventListener("dragover", e => {
    e.preventDefault()
    e.stopPropagation()
})

dropZone.addEventListener("dragenter", e => {
    e.preventDefault()
    e.stopPropagation()
})

dropZone.addEventListener("drop", e => {
    e.preventDefault()
    e.stopPropagation()
    webViewDropFiles("dropZone", e.dataTransfer.files);
})

btn1.onclick = async () => {
    var res = await webViewRequest("cmd1", {
        text: "Text",
        id: 123
    })
    console.log("cmd1", res)
}

btnClose.onclick = () => window.close()
btnMinimize.onclick = () => webViewMinimize()
btnRestore.onclick = () => webViewRestore()
btnMaximize.onclick = () => webViewMaximize()
btnDevTools.onclick = () => webViewShowDevTools()

btnWindowState.onclick = async () => {
    alert(`Window State: ${await webViewGetWindowState()}`)
}
