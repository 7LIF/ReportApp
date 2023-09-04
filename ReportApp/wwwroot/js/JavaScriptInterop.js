window.startRecording = async function (preview, recording, downloadButton, recordingTimeMS, dotnetRef) {
    const stream = preview.srcObject;
    const recorder = new MediaRecorder(stream);
    const recordedChunks = [];

    recorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
            recordedChunks.push(event.data);
        }
    };

    recorder.onstop = () => {
        const blob = new Blob(recordedChunks, { type: 'video/webm' });
        const reader = new FileReader();

        reader.onloadend = () => {
            const data = new Uint8Array(reader.result);
            dotnetRef.invokeMethodAsync('HandleRecordingData', data);
        };

        reader.readAsArrayBuffer(blob);
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

window.stopRecording = function (preview) {
    const stream = preview.srcObject;
    const tracks = stream.getTracks();

    tracks.forEach((track) => {
        track.stop();
    });
};

window.createObjectURL = function (blob) {
    return URL.createObjectURL(blob);
};
