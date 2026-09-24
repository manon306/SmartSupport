using Application.DTOs.Ticket;
using Application.Features.Ticket.Command.AssignTicket;
using Application.Features.Ticket.Command.ChangeStatus;
using Application.Features.Ticket.Command.CreateTicket;
using Application.Features.Ticket.Command.DeleteTicket;
using Application.Features.Ticket.Command.UpdateTicket;
using Application.Features.Ticket.Queries.GetTicketById;
using Application.Features.Ticket.Queries.GetTickets;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Creates a new support ticket for the currently authenticated user.
        /// </summary>
        /// <param name="dto">
        /// The ticket data including title, description, and priority.
        /// </param>
        /// <returns>
        /// The newly created support ticket.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateTicket(CreateTicketDto dto)
        {
            var result = await _mediator.Send(new CreateTicketCommand { Dto = dto });

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all support tickets available to the currently authenticated user.
        /// Employees can only view tickets they created, while Agents and Administrators
        /// can view all tickets.
        /// </summary>
        /// <returns>
        /// A collection of support tickets.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTickets()
        {
            var result = await _mediator.Send(new GetTicketsQuery());

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific support ticket by its identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the ticket.
        /// </param>
        /// <returns>
        /// The requested support ticket.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var result = await _mediator.Send(new GetTicketByIdQuery { Id = id});

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing support ticket.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the ticket to update.
        /// </param>
        /// <param name="dto">
        /// The updated ticket data including title, description, and priority.
        /// </param>
        /// <returns>
        /// The updated support ticket.
        /// </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTicket(int id, UpdateTicketDto dto)
        {
            var result = await _mediator.Send(new UpdateTicketCommand { Id = id, Dto = dto });

            return Ok(result);
        }

        /// <summary>
        /// Deletes an existing support ticket.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the ticket to delete.
        /// </param>
        /// <returns>
        /// A confirmation that the ticket was deleted successfully.
        /// </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _mediator.Send(new DeleteTicketCommand { Id = id });

            return Ok(new
            {
                message = "Ticket deleted successfully."
            });
        }

        /// <summary>
        /// Assigns a support ticket to an Agent.
        /// </summary>
        /// <param name="ticketId">
        /// The unique identifier of the ticket to assign.
        /// </param>
        /// <param name="agentId">
        /// The unique identifier of the Agent who will be assigned to the ticket.
        /// </param>
        /// <returns>
        /// The updated ticket with its assigned Agent.
        /// </returns>
        [HttpPut("{ticketId}/assign/{agentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignTicket(int ticketId, string agentId)
        {
            var result = await _mediator.Send(new AssignTicketCommand { TicketId = ticketId, AgentId = agentId });

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Changes the status of an existing support ticket.
        /// </summary>
        /// <param name="ticketId">
        /// The unique identifier of the ticket.
        /// </param>
        /// <param name="status">
        /// The new status of the ticket.
        /// </param>
        /// <returns>
        /// The updated support ticket with its new status.
        /// </returns>
        [HttpPut("{ticketId}/status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(
            int ticketId,
            TicketStatus status)
        {
            var result = await _mediator.Send(new ChangeStatusCommand { TicketId = ticketId, Status = status });

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            return Ok(result);
        }
    }
}
