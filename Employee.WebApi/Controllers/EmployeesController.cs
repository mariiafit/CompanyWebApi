﻿using CompanyWebApi.Resources;
using CompanyWebApi.Extensions;
using CompanyWebApi.Domains.Models;
using CompanyWebApi.Domains.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace CompanyWebApi.Controllers
{
    [Route("/api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<EmployeeResource>> GetAsync()
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var employees = await _employeeService.ListAsync(cancellationToken);
            var resources = _mapper.Map<List<Employee>, IEnumerable<EmployeeResource>>(employees);
            return resources;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveEmployeeResource resource)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessage());
            }

            var employee = _mapper.Map<SaveEmployeeResource, Employee>(resource);
            var result = await _employeeService.SaveAsync(employee, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var employeeResource = _mapper.Map<Employee, EmployeeResource>(result.Employee);

            return Ok(employeeResource);
        }
    }
}
