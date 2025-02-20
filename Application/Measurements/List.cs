using Application.Measurements;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;
public class List
{
    public class Query : IRequest<List<MeasurementDto>>
    {
    }

    public class Handler : IRequestHandler<Query, List<MeasurementDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MeasurementDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Measurements
                .ProjectTo<MeasurementDto>(_mapper.ConfigurationProvider) // ✅ Use DTO mapping
                .ToListAsync(cancellationToken);
        }
    }
}
