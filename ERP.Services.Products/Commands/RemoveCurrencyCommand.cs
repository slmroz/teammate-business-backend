using ERP.Services.Shared.CQRS;

namespace ERP.Services.Products.Commands;
public record RemoveCurrencyCommand(int Id) : ICommand;
