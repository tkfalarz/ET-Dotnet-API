using ET.WebAPI.Kernel.DomainModels;
using ET.WebAPI.Kernel.ErrorsHandling;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ET.WebAPI.Kernel.DomainServices
{
    public interface IReadingsService
    {
        Task<OperationResult> StoreReadingSetAsync(ReadingSet readingSet);
        Task<ReadingSet> GetLatestReadingsAsync(string deviceName);
        Task<ReadingSet> GetNearestLatestReadingsAsync(decimal latitude, decimal longitude);
        Task<IReadOnlyList<ReadingSet>> GetReadingsAsync(string deviceName, int limit);
        Task<Reading> GetTypedLatestReadingAsync(string deviceName, ReadingType readingType);
        Task<IReadOnlyList<Reading>> GetTypedReadingsAsync(string deviceName, ReadingType readingType, int limit);
    }
}