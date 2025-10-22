using AutoMapper;
using DataLifecycleManager.Domain.Entities;
using DataLifecycleManager.Presentation.ViewModels.SSISPackageExecution;
using System.Text.Json;

namespace DataLifecycleManager.Application.Mappings;

public class SSISPackageExecutionProfile : Profile
{
    public SSISPackageExecutionProfile()
    {
        CreateMap<SSISPackageExecution, ExecutionListViewModel>()
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.SSISPackage.PackageName))
            .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.SSISPackage.FolderName))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.SSISPackage.ProjectName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<SSISPackageExecution, ExecutionDetailsViewModel>()
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.SSISPackage.PackageName))
            .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.SSISPackage.FolderName))
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.SSISPackage.ProjectName))
            .ForMember(dest => dest.ServerAddress, opt => opt.MapFrom(src => src.SSISPackage.ServerAddress))
            .ForMember(dest => dest.CatalogName, opt => opt.MapFrom(src => src.SSISPackage.CatalogName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ExecutionParameters, opt => opt.MapFrom(src => DeserializeParameters(src.ExecutionParameters)));
    }

    private static Dictionary<string, string> DeserializeParameters(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json, (JsonSerializerOptions?)null) 
                   ?? new Dictionary<string, string>();
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
