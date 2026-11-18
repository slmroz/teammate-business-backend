using ERP.Services.Sales.Commands;
using ERP.Services.Sales.Queries;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using ERP.Services.Shared.Search;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuoteController : ControllerBase
{
    private readonly ICommandHandler<AddQuoteCommand> _addQuoteHandler;
    private readonly ICommandHandler<UpdateQuoteCommand> _updateQuoteHandler;
    private readonly ICommandHandler<RemoveQuoteCommand> _removeQuoteHandler;
    private readonly IQueryHandler<GetQuoteQuery, QuoteDto> _getQuoteHandler;
    private readonly IQueryHandler<GetQuotesQuery, PagedResult<QuoteDto>> _getQuotesHandler;

    public QuoteController(
        ICommandHandler<AddQuoteCommand> addQuoteHandler,
        ICommandHandler<UpdateQuoteCommand> updateQuoteHandler,
        ICommandHandler<RemoveQuoteCommand> removeQuoteHandler,
        IQueryHandler<GetQuoteQuery, QuoteDto> getQuoteHandler,
        IQueryHandler<GetQuotesQuery, PagedResult<QuoteDto>> getQuotesHandler)
    {
        _addQuoteHandler = addQuoteHandler;
        _updateQuoteHandler = updateQuoteHandler;
        _removeQuoteHandler = removeQuoteHandler;
        _getQuoteHandler = getQuoteHandler;
        _getQuotesHandler = getQuotesHandler;
    }

    /// <summary>
    /// Pobierz pojedynczą ofertę z danymi klienta i pozycjami
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuoteDto>> Get(int id)
    {
        var quote = await _getQuoteHandler.HandleAsync(new GetQuoteQuery(id));
        return Ok(quote);
    }

    /// <summary>
    /// Paginowana lista ofert z zaawansowanym filtrowaniem i sortowaniem
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<QuoteDto>>> GetList(
        [FromQuery] int? customerId = null,
        [FromQuery] string? quoteNumber = null,
        [FromQuery] int? quoteStatusId = null,
        [FromQuery] DateTime? validUntilFrom = null,
        [FromQuery] DateTime? validUntilTo = null,
        [FromQuery] decimal? minTotalAmount = null,
        [FromQuery] decimal? maxTotalAmount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = "desc")
    {
        var result = await _getQuotesHandler.HandleAsync(new GetQuotesQuery(
            page, pageSize, customerId, quoteNumber, quoteStatusId,
            validUntilFrom, validUntilTo, minTotalAmount, maxTotalAmount,
            sortBy, sortDirection));
        return Ok(result);
    }

    /// <summary>
    /// Dodaj nową ofertę (automatycznie recalculate totals)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AddQuoteCommand command)
    {
        await _addQuoteHandler.HandleAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, command);
    }

    /// <summary>
    /// Aktualizuj ofertę (recalculate totals po zmianie pozycji)
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuoteCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _updateQuoteHandler.HandleAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Usuń ofertę (soft delete - tylko bez aktywnych pozycji)
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, [FromQuery] int removedBy)
    {
        await _removeQuoteHandler.HandleAsync(new RemoveQuoteCommand(id, removedBy));
        return NoContent();
    }
}
