using NewsBot.Models.Base;
using NewsBot.Models.ViewModels;

namespace NewsBot.Services.Interfaces
{
    public interface INewsService
    {
        public Task<ServiceResult> GetNewsById(int id,CancellationToken cancellationToken);
        public Task<ServiceResult> GetNews( CancellationToken cancellationToken);
        public Task<ServiceResult> PostNews(NewsViewModel model, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateNews(NewsUpdateViewModel model, CancellationToken cancellationToken);
        public Task<ServiceResult> DeleteNews(int id, CancellationToken cancellationToken);
    }
}
