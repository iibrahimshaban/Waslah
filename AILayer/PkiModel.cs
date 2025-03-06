using System;
using System.IO;
using System.Runtime.InteropServices;
using IronPython.Hosting;
using Mono.Unix;
using Python.Runtime;


public class PkiModel : IDisposable
{
    private dynamic _model;

    public PkiModel(string modelPath)
    {
        // Initialize Python engine
        PythonEngine.Initialize();
        using (Py.GIL()) // Acquire the Python Global Interpreter Lock
        {
            // Import necessary Python libraries
            dynamic pickle = Py.Import("pickle");

            // Load the model from the .pkl file
            using (var fs = new FileStream(modelPath, FileMode.Open))
            {
                _model = pickle.load(fs);
            }
        }
    }

    public float Predict(float[] features)
    {
        using (Py.GIL())
        {
            // Convert the input array to a Python list
            dynamic pyFeatures = new PyList(features.Select(f => new PyFloat(f)).ToArray());

            // Make a prediction using the model
            dynamic prediction = _model.predict(pyFeatures);

            // Return the predicted price
            return (float)prediction[0];
        }
    }

    public void Dispose()
    {
        PythonEngine.Shutdown();
    }
}