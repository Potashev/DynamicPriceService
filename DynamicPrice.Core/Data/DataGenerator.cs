using Bogus;
using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Data;

//public static class DataGenerator
//{
//	public static List<Product> GenerateProducts(
//		int companyId,
//		int count)
//	{
//		var faker = new Faker<Product>("ru")
//			.RuleFor(p => p.CompanyId, companyId)
//			.RuleFor(p => p.Title, f => f.Commerce.ProductName())
//			.RuleFor(p => p.Price, f => f.Random.Decimal(100, 10000))
//			.RuleFor(p => p.MinimumPrice, (f, p) => p.Price * 0.5m)
//			.RuleFor(p => p.Quantity, f => f.Random.Int(1, 500))
//			.RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
//			.RuleFor(p => p.LastSellTime, f => f.Date.Recent(30));

//		return faker.Generate(count);
//	}
//}

