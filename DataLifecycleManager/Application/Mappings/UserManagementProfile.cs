using AutoMapper;
using DataLifecycleManager.Application.DTOs.UserManagement;
using DataLifecycleManager.Presentation.ViewModels.UserManagement;

namespace DataLifecycleManager.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for User Management mappings
    /// </summary>
    public class UserManagementProfile : Profile
    {
        public UserManagementProfile()
        {
            // DTO to ViewModel mappings
            CreateMap<UserDto, UserViewModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => string.Join(", ", src.Roles)));

            CreateMap<UserDto, EditUserViewModel>()
                .ForMember(dest => dest.SelectedRoles, opt => opt.MapFrom(src => src.Roles));

            // ViewModel to DTO mappings
            CreateMap<CreateUserViewModel, CreateUserDto>();
            CreateMap<EditUserViewModel, UpdateUserDto>();
        }
    }
}
