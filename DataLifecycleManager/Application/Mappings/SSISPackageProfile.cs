using AutoMapper;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Presentation.ViewModels.SSISPackage;

namespace DataLifecycleManager.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for SSIS Package mappings
    /// </summary>
    public class SSISPackageProfile : Profile
    {
        public SSISPackageProfile()
        {
            // Entity to ViewModel mappings
            CreateMap<SSISPackage, SSISPackageViewModel>();
            CreateMap<SSISPackage, EditSSISPackageViewModel>();
            CreateMap<SSISPackage, ExecuteSSISPackageViewModel>();

            // ViewModel to Entity mappings
            CreateMap<CreateSSISPackageViewModel, SSISPackage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastExecutionDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastExecutionStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Executions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<EditSSISPackageViewModel, SSISPackage>()
                .ForMember(dest => dest.LastExecutionDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastExecutionStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Executions, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
