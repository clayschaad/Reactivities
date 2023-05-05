using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this.mapper = mapper;
                this.context = context;
                this.userAccessor = userAccessor;
            }

            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = context.Activities
                    .Where(a => a.Date >= request.Params.StartDate)
                    .OrderBy(a => a.Date)
                    .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUsername = userAccessor.GetUsername() })
                    .AsQueryable();

                if (request.Params.IsGoing) 
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == userAccessor.GetUsername()));
                }

                if (request.Params.IsHost) 
                {
                    query = query.Where(x => x.HostUsername == userAccessor.GetUsername());
                }

                var pagedList = await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize);
                return Result<PagedList<ActivityDto>>.Success(pagedList);
            }
        }
    }
}