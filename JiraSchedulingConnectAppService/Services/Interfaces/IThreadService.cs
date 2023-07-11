using ModelLibrary.DTOs.Thread;

namespace JiraSchedulingConnectAppService.Services.Interfaces
{
    public interface IThreadService
    {
        public ThreadModel GetThreadModel(int threadId);
        public int StartThread(ThreadStart threadStart);
        public ThreadResultDTO GetThreadResult(int threadId);
    }
}
