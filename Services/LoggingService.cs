using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UiPath.CustomProxy.Contracts;
using UiPath.CustomProxy.ViewModels;

namespace UiPath.CustomProxy.Services
{
    internal class LoggingService : ILoggingService
    {
        private readonly ConcurrentQueue<string> _queue = new();

        public void Init(MainWindowViewModel mainWindowViewModel) => Task.Run(() => ProcessQueue(mainWindowViewModel));

        public void Log(string message) => _queue.Enqueue(message);

        private void ProcessQueue(MainWindowViewModel mainWindowViewModel)
        {
            while (true)
            {
                if (!mainWindowViewModel.IsWindowReady || _queue.IsEmpty)
                    continue;

                if (_queue.TryDequeue(out var result))
                    ProcessQueueInternal(mainWindowViewModel, result);
            }
        }

        private static void ProcessQueueInternal(MainWindowViewModel mainWindowViewModel, string result)
        {
            try
            {
                var currentDateTime = DateTime.Now;
                string formattedDateTime = currentDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                var message = $"[{formattedDateTime}] {result}";
                LogToFile(message);

                if (message.Length > 150)
                    message = message[..150] + " ...";
                mainWindowViewModel.Log(message + "\n");
            }
            catch
            {
            }
        }

        private static void LogToFile(string message)
        {
            using var logStream = new StreamWriter(GetLogStream(), Encoding.UTF8);
            logStream.WriteLine(message);
        }

        private static FileStream GetLogStream() => new(GetLogFileName(), FileMode.Append, FileAccess.Write);

        private static string GetLogFileName()
        {
            var currentLogFileName = $"{DateTime.Now:yyyy-MM-dd}_UiPath.CustomProxy.log";
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\UiPath\Logs\";
            return Path.Combine(folderPath, currentLogFileName);
        }
    }
}
