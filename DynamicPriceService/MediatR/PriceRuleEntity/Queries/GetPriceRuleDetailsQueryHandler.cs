using AutoMapper;
using DynamicPriceService.Data;
using DynamicPriceService.MediatR.ViewModel;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQueryHandler
    : IRequestHandler<GetPriceRuleDetailsQuery, PriceRuleViewModel>
{
    private readonly DynamicPriceServiceContext _context;
    private readonly IMapper _mapper;

    public GetPriceRuleDetailsQueryHandler(DynamicPriceServiceContext context, IMapper mapper)
        => (_context, _mapper) = (context, mapper);

    public async Task<PriceRuleViewModel> Handle(GetPriceRuleDetailsQuery request, CancellationToken cancellationToken)
    {
        var company = GetCompany();
        var priceRule = await _context.PriceRule
                .FirstOrDefaultAsync(pr => pr.PriceRuleId == company.CompanyId);

        priceRule ??= AddDefaultRule(company);

        var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);
        return priceRuleVm;
    }

    private Company GetCompany() => _context.Company.FirstOrDefault();

    private PriceRule AddDefaultRule(Company company)
    {
        var priceRule = new PriceRule
        {
            Increase = 10,
            Reduction = 1,
            NoSellTime = new TimeSpan(0, 0, 10),
            Company = company
        };

        // concept conflict - write action in query
        _context.Add(priceRule);
        _context.SaveChanges();

        //PriceRuleId lost
        return priceRule;
    }

}
