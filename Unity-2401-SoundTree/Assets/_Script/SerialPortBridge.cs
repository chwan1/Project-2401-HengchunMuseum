using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.IO.Ports;
using System;

public class SerialPortBridge : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;

    Task task;
    // ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
    CancellationTokenSource cts = new CancellationTokenSource();
    ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        task = Task.Run(() => ReceiveData(cts.Token), cts.Token);
    }

    // Update is called once per frame
    void Update()
    {
        if (actions.TryDequeue(out var action))
            action();
    }

    void OnDisable()
    {
        cts.Cancel();
        task?.Wait();
        task?.Dispose();
    }

    async void ReceiveData(CancellationToken token)
    {
        try
        {
            await Task.Delay(1000);
            var port = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            port.Open();

            while (token.IsCancellationRequested == false)
            {
                if (port.BytesToRead > 0)
                {
                    var message = port.ReadExisting();
                    actions.Enqueue(() => Debug.Log(message));
                }
                await Task.Delay(20);
            }
            actions.Enqueue(() => Debug.Log("Closing port"));
            port.Close();
        }
        catch (Exception e)
        {
            actions.Enqueue(() => Debug.Log(e.Message));
        }
    }
}
