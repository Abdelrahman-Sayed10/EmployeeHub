using AutoMapper;
using EmployeeHub.Domain.Entities.Operation;
using EmployeeHub.Dtos.DepartmentDto;
using EmployeeHub.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Department
        CreateMap<Department, DepartmentDto>().ReverseMap();

        CreateMap<DepartmentAddDto, DepartmentDto>().ReverseMap();

        CreateMap<Department, DepartmentAddDto>().ReverseMap();


        // Employee
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.DepartmentDescription,
                        opt => opt.MapFrom(src => src.Department.Description));

        CreateMap<Employee, EmployeeAddDto>().ReverseMap();
        CreateMap<EmployeeDto, EmployeeAddDto>().ReverseMap();
    }

}
