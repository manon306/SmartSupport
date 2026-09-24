using Application.DTOs.Ticket;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class TicketProfile :Profile
    {
        public TicketProfile()
        {
            CreateMap<CreateTicketDto, Ticket>();
            CreateMap<Ticket, TicketResponseDto>();
        }
    }
}
