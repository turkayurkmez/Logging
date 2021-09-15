using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomizeLogging.Infrastructure
{
    public class ColoredConsoleLoggerConfiguration 
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
        public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;

    }

    public class ColoredConsoleLoggerProvider : ILoggerProvider
    {
        private ColoredConsoleLoggerConfiguration config;

        private ConcurrentDictionary<string, ColoredConsoleLogger> loggers = new ConcurrentDictionary<string, ColoredConsoleLogger>();

        public ColoredConsoleLoggerProvider(ColoredConsoleLoggerConfiguration config)
        {
            this.config = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new ColoredConsoleLogger(config,name));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }

    public class ColoredConsoleLogger : ILogger
    {
        private static object lockForThread = new object();
        private ColoredConsoleLoggerConfiguration config;
        private string name;

        public ColoredConsoleLogger(ColoredConsoleLoggerConfiguration configuration, string name)
        {
            this.config = configuration;
            this.name = name;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            lock (lockForThread)
            {
                if (config.EventId == 0 || config.EventId == eventId.Id)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = config.Color;
                    Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {name} - {formatter(state,exception)}");
                    Console.ForegroundColor = color;
                }

            }
        }
    }
}
