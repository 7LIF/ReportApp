window.startRecording = async function (preview, recording, downloadButton, recordingTimeMS) {
    const stream = preview.srcObject;
    const recorder = new MediaRecorder(stream);
    const recordedChunks = [];

    recorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
            recordedChunks.push(event.data);
        }
    };

    recorder.start();

    preview.captureStream = preview.captureStream || preview.mozCaptureStream;

    await new Promise((resolve) => {
        preview.onplaying = resolve;
    });

    setTimeout(async () => {
        if (recorder.state == "recording") {
            recorder.stop();
        }
    }, recordingTimeMS);
};

window.addDataAvailableCallback = function (stream, dotnetRef) {
    const recorder = new MediaRecorder(stream);

    recorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
            const reader = new FileReader();
            reader.onloadend = () => {
                const data = new Uint8Array(reader.result);
                dotnetRef.invokeMethodAsync('HandleRecordingData', data);
            };
            reader.readAsArrayBuffer(event.data);
        }
    };
};

window.stopRecording = function (preview) {
    const stream = preview.srcObject;
    stream.getTracks().forEach((track) => {
        track.stop();
    });
};

window.stopStream = function (preview) {
    const stream = preview.srcObject;
    stream.getTracks().forEach((track) => {
        track.stop();
    });
    preview.srcObject = null;
};


window.createObjectURL = function (blob) {
    return URL.createObjectURL(blob);
};


window.createBlobAndGetObjectURL = function (data, type) {
    var blob = new Blob([data], { type: type });
    var objectURL = URL.createObjectURL(blob);
    return objectURL;
};

