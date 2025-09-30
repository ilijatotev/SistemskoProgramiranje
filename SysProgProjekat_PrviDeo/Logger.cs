using System;
using System.Threading;

public class Logger
{
    private static readonly object lockObject = new object();

    public void Log(string message)
    {
        lock (lockObject)
        {
            Console.WriteLine(message);
        }
    }
}