using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Channels;
using AutoMapper;
using JiraSchedulingConnectAppService.Repository;
using JiraSchedulingConnectAppService.Repository.Interfaces;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;

namespace JiraSchedulingConnectAppService.Services
{
    public class DatabaseLoggerService : BackgroundService, ILoggerService
    {
        private readonly Channel<LogMessage> logMessageQueue;
        private readonly IHostApplicationLifetime HostApplicationLifetime;
        private const int MAX_BATCH_SIZE = 10;
        private readonly ILogRepository LogRepository;

        public DatabaseLoggerService(ILogRepository logRepository, IHostApplicationLifetime hostApplicationLifetime)

		{
            logMessageQueue = Channel.CreateUnbounded<LogMessage>();
            LogRepository = logRepository;
            HostApplicationLifetime = hostApplicationLifetime;

        }

        public void Log(LogLevel logLevel, Exception exception )
        {

            var logMessage = new LogMessage()
            {
                LogLevel = logLevel.ToString(),
                Message = exception.Message,
                ExceptionSource = exception?.StackTrace,
                ExceptionType = exception?.GetType().ToString(),
                ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId,
                Timestamp = DateTimeOffset.Now
            };

            if (!logMessageQueue.Writer.TryWrite(logMessage))
            {
                throw new InvalidOperationException("Failed to write the log message");
            }
        }

        public async override System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }


        protected async override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    Console.WriteLine("Waiting for log messages");
                    var batch = await GetBatch(stoppingToken);

                    Console.WriteLine($"Got a batch with {batch.Count}(s) log messages. Bulk inserting them now.");

                    //Let non-retryable errors from this bubble up and crash the service
                    await LogRepository.Insert(batch);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Stopping token was canceled, which means the service is shutting down.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fatal exception in database logger. Crashing service. Error={ex}");
                    HostApplicationLifetime.StopApplication();
                    return;
                }
            }
        }

        private async Task<List<LogMessage>> GetBatch(CancellationToken cancellationToken)
        {
            await logMessageQueue.Reader.WaitToReadAsync(cancellationToken);

            var batch = new List<LogMessage>();

            while (batch.Count < MAX_BATCH_SIZE && logMessageQueue.Reader.TryRead(out LogMessage message))
            {
                batch.Add(message);
            }

            return batch;
        }

    }
}

