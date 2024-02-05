using Microsoft.AspNetCore.Mvc;
using NewsBot.Common.Resources;
using NewsBot.Models.Base;
using NewsBot.Models.ViewModels;
using NewsBot.Services.Interfaces;
using NLog;
using System;
using System.Net.Mime;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/news")]
    public class NewsController : APIControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNews(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _newsService.GetNews(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain);
            }
        }

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNewsById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _newsService.GetNewsById(id, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain);
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostNews([FromBody] NewsViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _newsService.PostNews(model, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain);
            }
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsUpdateViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                model.Id = id; // Assuming the id is also provided in the request body
                var result = await _newsService.UpdateNews(model, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain);
            }
        }

        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResult), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteNews(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _newsService.DeleteNews(id, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain);
            }
        }
    }
}
