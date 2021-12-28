using AutoMapper;
using ProductManagementApi.Dtos;
using ProductManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Configuration
{
    public class RuntimeProfile : Profile
    {   
        public RuntimeProfile()
        {
            CreateMap<AspNetUserModel, AspNetUser>()
                   .ForMember(dest => dest.Id, option => option
                   .MapFrom(src => new AspNetUser
                   {
                       FirstName = src.FirstName,
                       LastName = src.LastName,
                       UserName = src.UserName ?? (src.FirstName + src.LastName).ToLower(),
                       Email = src.Email,
                       EmailConfirmed = false,
                       PhoneNumber = src.PhoneNumber,
                       PhoneNumberConfirmed = false,
                       Password = src.Password,
                       RoleName = src.RoleName,
                       Address = src.Address
                   })).ReverseMap();

            CreateMap<ProductModel, Product>()
                .ForMember(detinationMember => detinationMember.Id, option => option
                .MapFrom(sourceMember => sourceMember.Id ?? Guid.NewGuid().ToString()))
                .ReverseMap();
        }
    }
}
