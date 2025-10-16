using AutoMapper;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Presentation.ViewModels.DatabaseConnection;

namespace DataLifecycleManager.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for DatabaseConnection mappings
    /// </summary>
    public class DatabaseConnectionProfile : Profile
    {
        public DatabaseConnectionProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<DatabaseConnection, DatabaseConnectionViewModel>();
            CreateMap<DatabaseConnection, EditDatabaseConnectionViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Don't map encrypted password back

            // ViewModel to Entity mappings
            CreateMap<CreateDatabaseConnectionViewModel, DatabaseConnection>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EncryptedPassword, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<EditDatabaseConnectionViewModel, DatabaseConnection>()
                .ForMember(dest => dest.EncryptedPassword, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
