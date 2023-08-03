using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DTOs.Thread;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class ThreadService : IThreadService
    {
        // Thread with random string key
        private static Dictionary<string, ThreadModel> threadDict = new Dictionary<string, ThreadModel>();

        public ThreadService()
        {
        }

        public ThreadModel GetThreadModel(string threadId)
        {
            if (threadDict.ContainsKey(threadId))
            {
                return threadDict[threadId];
            }
            else
            {
                throw new NotFoundException($"Can not find thread with id: {threadId}");
            }
        }

        public static string CreateThreadId()
        {
            string? threadId;
            do
            {
                threadId = Utils.RandomString(Const.THREAD_ID_LENGTH);
            } while (threadDict.ContainsKey(threadId));
            threadDict.Add(threadId, new ThreadModel(threadId));
            return threadId;
        }

        public string StartThread(string threadId, ThreadStart threadStart)
        {
            Thread thread = new Thread(threadStart);
            thread.Start();
            return threadId;
        }

        public ThreadResultDTO GetThreadResult(string threadId)
        {
            if (threadDict.ContainsKey(threadId))
            {
                var thread = threadDict[threadId];
                // Clean a thread data if it finish and allow read onetime
                switch (thread.Status)
                {
                    case Const.THREAD_STATUS.SUCCESS:
                        threadDict.Remove(threadId);
                        return new ThreadResultDTO()
                        {
                            ThreadId = thread.ThreadId,
                            Status = thread.Status,
                            Result = thread.Result,
                        };
                    case Const.THREAD_STATUS.ERROR:
                        threadDict.Remove(threadId);
                        return new ThreadResultDTO()
                        {
                            ThreadId = thread.ThreadId,
                            Status = thread.Status,
                            Result = thread.Result,
                        };
                    default:
                        return new ThreadResultDTO()
                        {
                            ThreadId = threadId,
                            Status = thread.Status,
                            Progress = thread.Progress
                        };
                }

            }
            else
            {
                throw new NotFoundException($"Can not find thread with id: {threadId}");
            }
        }
    }
}
