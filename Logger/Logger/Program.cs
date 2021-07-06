using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Logger
{
    class Program
    {
        internal enum Sevirity
        {
            Trace,
            Debug,
            Information,
            Warning,
            Error,
            Critical,
            Severity
        }
        
        internal class Logger : IDisposable
        {
            private readonly StreamWriter _logWriter;
    
            public Logger(string pathFile)
            {
                _logWriter = new StreamWriter(pathFile);
            }
    
            public void Log(string logMessage, Sevirity sevirity)
            {
                _logWriter.WriteLine($"[{DateTime.Now:G}]{sevirity}{logMessage}");
            }
    
            public void Dispose()
            {
                _logWriter.Dispose();
                GC.SuppressFinalize(this);
            }
    
            ~Logger()
            {
                _logWriter.Dispose();
            }
    
        }

        public class LogContext : IDisposable
        {
            private readonly Logger _d;

            public LogContext(string filePath)
            {
                _d = new Logger(filePath);
            }

            public void Log(string message, Sevirity sevirity)
            {
                _d.Log(message, sevirity);
            }
            
            public void Dispose()
            {
                _d.Dispose();
            }
        }
        
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Wrong num param!");
            }

            using (var logContext = new LogContext(args[0]))
            {
                logContext.Log("Bring me the pizza", Sevirity.Trace);
                logContext.Log("Russky Cockney", Sevirity.Error);
                logContext.Log("Something else", Sevirity.Critical);
            }
        }
    }
}