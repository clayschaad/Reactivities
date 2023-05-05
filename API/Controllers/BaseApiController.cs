using API.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator mediator;

        protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null)
            {
                return NotFound();
            }
            if (result.IsSuccess && result.Value != null)
            {
                return Ok(result.Value);
            }
            if (result.IsSuccess && result.Value == null)
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
        {
            if (result == null)
            {
                return NotFound();
            }
            if (result.IsSuccess && result.Value != null)
            {
                Response.AddPaginationHeader(
                    currentPage: result.Value.CurrentPage,
                    itemsPerPage: result.Value.PageSize,
                    totalItems: result.Value.TotalCount,
                    totalPages: result.Value.TotalPages);
                return Ok(result.Value);
            }
            if (result.IsSuccess && result.Value == null)
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }
    }
}