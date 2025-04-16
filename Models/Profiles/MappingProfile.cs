using AutoMapper;
using LeaveManagement.Models.Datas;
using LeaveManagement.Models.DTOs;

namespace LeaveManagement.Models.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestOutputDTO>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<LeaveRequestInputDTO, LeaveRequest>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.LeaveType)
                        ? Enum.Parse<LeaveType>(src.LeaveType)
                        : LeaveType.Default))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => LeaveStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
