using ERP.Services.Shared.CQRS;

namespace ERP.Services.Products.Commands;
public record RemoveProductGroupCommand(int Id) : ICommand;

