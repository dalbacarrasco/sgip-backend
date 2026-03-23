using Application.Loans.Commands.ApplyLoan;
using Application.Loans.Commands.ApproveLoan;
using Application.Loans.Commands.SimulateLoan;
using Application.Loans.Queries.GetLoan;
using Application.Loans.Queries.GetPaymentSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/v1/loans")]
    public class LoansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> Simulate([FromBody] SimulateLoanCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyLoanCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.LoanId }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetLoanQuery(id));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPatch("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var approved = await _mediator.Send(new ApproveLoanCommand(id));
            return Ok(new { approved });
        }

        [HttpGet("{id:guid}/payment-schedule")]
        public async Task<IActionResult> GetPaymentSchedule(Guid id)
        {
            var result = await _mediator.Send(new GetPaymentScheduleQuery(id));
            return Ok(result);
        }
    }
}