using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;
using ModelLibrary.DTOs.Thread;
using UtilsLibrary.Exceptions;
using static System.Net.WebRequestMethods;

namespace JiraSchedulingConnectAppService.Services
{
    public class ThreadService : IThreadService
    {
        private static Dictionary<int, ThreadModel> _threads = new Dictionary<int, ThreadModel>();
        private static int _nextThreadId = Const.THREAD_ID_COUNT_START;

        public ThreadService()
        {
        }

        public ThreadModel GetThreadModel(int threadId)
        {
            if (_threads.ContainsKey(threadId))
            {
                return _threads[threadId];
            }
            else
            {
                throw new NotFoundException($"Can not find thread with id: {threadId}");
            }
        }

        public int StartThread(ThreadStart threadStart)
        {

            int threadId = _nextThreadId++;
            Thread thread = new Thread(threadStart);
            thread.Start();

            _threads[threadId] = new ThreadModel
            {
                ThreadId = threadId,
                Status = Const.THREAD_STATUS.RUNNING
            };

            return threadId;
        }

        public ThreadResultDTO GetThreadResult(int threadId)
        {
            if (_threads.ContainsKey(threadId))
            {
                var thread = _threads[threadId];
                switch (thread.Status)
                {
                    case Const.THREAD_STATUS.SUCCESS:
                        return new ThreadResultDTO()
                        {
                            Status = thread.Status,
                            Result = thread.Result
                        };
                    case Const.THREAD_STATUS.ERROR:
                        return new ThreadResultDTO()
                        {
                            Status = thread.Status,
                            Result = thread.Result
                        };
                    default:
                        return new ThreadResultDTO()
                        {
                            Status = thread.Status,
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
