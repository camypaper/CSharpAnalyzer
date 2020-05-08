using System;

namespace CSharpAnalyzer.Logging
{
    public enum LoggingLevel
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    public static class Logger
    {
        static LoggingLevel Level { get; set; } = LoggingLevel.Info;
        static void log(LoggingLevel level, string message)
        {
            if (Level <= level)
                Console.Error.WriteLine($"[{level.ToString()}]\t{DateTime.Now:u}\t{message}");
        }
        public static void Info(string message) => log(LoggingLevel.Info, message);
        public static void Warn(string message) => log(LoggingLevel.Warn, message);
        public static void Error(string message) => log(LoggingLevel.Error, message);
    }
}
