using ERP.Services.Shared.CQRS;
using ERP.Services.Sales.Commands;
using ERP.Services.Sales.Queries;
using Microsoft.AspNetCore.Mvc;
using ERP.Services.Shared.DTO;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteItemController : ControllerBase
{
    private readonly ICommandHandler<AddQuoteItemCommand> _addQuoteItemHandler;
    private readonly ICommandHandler<UpdateQuoteItemCommand> _updateQuoteItemHandler;
    private readonly ICommandHandler<RemoveQuoteItemCommand> _removeQuoteItemHandler;
    private readonly IQueryHandler<GetQuoteItemsQuery, List<QuoteItemDto>> _getQuoteItemsHandler;

    public QuoteItemController(
        ICommandHandler<AddQuoteItemCommand> addQuoteItemHandler,
        ICommandHandler<UpdateQuoteItemCommand> updateQuoteItemHandler,
        ICommandHandler<RemoveQuoteItemCommand> removeQuoteItemHandler,
        IQueryHandler<GetQuoteItemsQuery, List<QuoteItemDto>> getQuoteItemsHandler)
    {
        _addQuoteItemHandler = addQuoteItemHandler;
        _updateQuoteItemHandler = updateQuoteItemHandler;
        _removeQuoteItemHandler = removeQuoteItemHandler;
        _getQuoteItemsHandler = getQuoteItemsHandler;
    }

    /// <summary>
    /// Pobierz wszystkie pozycje oferty z nazwami produktów
    /// </summary>
    [HttpGet("quote/{quoteId:int}")]
    public async Task<ActionResult<List<QuoteItemDto>>> GetByQuote(int quoteId)
    {
        var items = await _getQuoteItemsHandler.HandleAsync(new GetQuoteItemsQuery(quoteId));
        return Ok(items);
    }

    /// <summary>
    /// Dodaj pozycję do oferty (automatycznie recalculate quote totals)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AddQuoteItemCommand command)
    {
        await _addQuoteItemHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetByQuote),
            new { quoteId = command.QuoteId }, command);
    }

    /// <summary>
    /// Aktualizuj pozycję oferty (recalculate totals)
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuoteItemCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateQuoteItemHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń pozycję oferty (soft delete + recalculate totals)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, [FromQuery] int removedBy)
    {
        await _removeQuoteItemHandler.HandleAsync(new RemoveQuoteItemCommand(id, removedBy));
        return NoContent();
    }
}
