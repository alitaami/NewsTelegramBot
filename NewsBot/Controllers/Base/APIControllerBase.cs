﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using NewsBot.Models.Base;

[ApiController]
public class APIControllerBase : ControllerBase
{
    protected IActionResult APIResponse(ServiceResult serviceResult)
    {
        if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.OK)
        {
            var res = new ServiceResult(serviceResult.Data, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, null, null));

            if (serviceResult.Data == null)
                return Ok();
            else
                return Ok(res);
        }

        else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.BadRequest)
        {
            return BadRequest(serviceResult);
        }
        else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.NotFound)
        {
            return NotFound(serviceResult);
        }
        else if (serviceResult.Result.HttpStatusCode == (int)HttpStatusCode.InternalServerError)
        {
            // Return a new ServiceResult with null data for internal server errors
            return StatusCode((int)HttpStatusCode.InternalServerError, new ServiceResult(null, serviceResult.Result));
        }
        else
        {
            // Handle other cases, if needed
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }

    protected IActionResult InternalServerError()
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(ErrorCodeEnum.InternalError, null)));
    }

    protected IActionResult InternalServerError(ErrorCodeEnum error)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(error, null)));
    }

    protected IActionResult InternalServerError(string message)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(ErrorCodeEnum.InternalError, message)));
    }

    protected IActionResult InternalServerError(ErrorCodeEnum error, string message)
    {
        return APIResponse(new ServiceResult(null, CreateInternalErrorResult(error, message)));
    }

    private ApiResult CreateInternalErrorResult(ErrorCodeEnum error, string? message)
    {
        return new ApiResult(HttpStatusCode.InternalServerError, error, message, null);
    }
}
