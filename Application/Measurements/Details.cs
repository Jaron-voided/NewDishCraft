using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;

public class Details
{
    public class Query : IRequest<Result<MeasurementDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<MeasurementDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<MeasurementDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var measurement = await _context.Measurements
                .Where(m => m.Id == request.Id) // ✅ Apply filtering first (better performance)
                .Include(m => m.Ingredient)  // ✅ Ensure Ingredient is loaded
                .Include(m => m.Recipe)      // ✅ Ensure Recipe is loaded
                .ProjectTo<MeasurementDto>(_mapper.ConfigurationProvider) // ✅ Map to DTO
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (measurement == null) return Result<MeasurementDto>.Failure("Measurement not found");

            return Result<MeasurementDto>.Success(measurement);
        }
    }
}
