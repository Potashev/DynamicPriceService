namespace DynamicPrice.Core.Rabbit;

public record CompanyMonitoringStarted(int CompanyId);
public record CompanyMonitoringStopped(int CompanyId);
public record PriceReduceMessage(int ProductId, int CompanyId);	//todo: think about remove companyId from message
public record CompanyPayload(int CompanyId);
