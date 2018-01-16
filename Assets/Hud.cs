using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.WebCam;

public class Hud : MonoBehaviour {
    public Text DiagnosticPanel;

    public Text IdentifiedStatus;
    public void SetIdentificationStatus(bool state)
    {
        IdentifiedStatus.text = state ? "IDENTIFIED" : "";
        IdentifiedStatus.color = Color.green;
    }

    public Text Name;
    public void SetName(string name)
    {
        Name.text = "Name: " + name;
    }

    public Text Age;
    public void SetAge(int age)
    {
        Age.text = "Age: " + age.ToString();
    }

    private System.Threading.Timer _timer;

    // Use this for initialization
    void Start () {
        SetIdentificationStatus(true);
        SetName("Kane Penley");
        SetAge(23);
        DiagnosticPanel.text = "Loading...";

        // The scene is checked for faces at each interval.
        int interval = 2;
        InvokeRepeating("Tick", 0, interval);

    }

    private void Tick()
    {
        AnalyzeScene();
    }

    private void AnalyzeScene()
    {
        DiagnosticPanel.text = "Analyzing Scene...";
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    PhotoCapture _photoCaptureObject = null;


    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters
        {
            hologramOpacity = 0.0f,
            cameraResolutionWidth = cameraResolution.width,
            cameraResolutionHeight = cameraResolution.height,
            pixelFormat = CapturePixelFormat.BGRA32
        };

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"image.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            _photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
        }
        else
        {
            DiagnosticPanel.text = "DIAGNOSTIC\n**************\n\nUnable to start photo mode.";
        }
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format(@"image.jpg");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            byte[] image = File.ReadAllBytes(filePath);
            string person = GetRecognizedPerson(image);
            Name.text = person;
        }
        else
        {
            DiagnosticPanel.text = "DIAGNOSTIC\n**************\n\nFailed to save Photo to disk.";
        }
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    private string GetRecognizedPerson(byte[] image)
    {
        // This is where we would place facial recognition code.
        DiagnosticPanel.text = "Looking for recognized faces.";
        return "Kane Penley";
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
