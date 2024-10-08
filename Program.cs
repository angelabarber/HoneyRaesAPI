using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;
using Microsoft.AspNetCore.Authentication;

List<Customer> customers = new List<Customer>
{
    new Customer()
     {
        Id = 1,
        Name = "Glidden Masters",
        Address = "2802 Zula Locks Dr"
    },
    new Customer()
    {
        Id = 2,
        Name = "Filipe Gonzaga",
        Address = "56849 Fadel Gateway"
    },
    new Customer()
    {
        Id = 3,
        Name = "Roger Talos",
        Address = "7346 Ritchie Road"
    }
};

List<Employee> employees = new List<Employee>
{
    new Employee()
    {
        Id = 1,
        Name = "Willy Bender",
        Specialty = "Macs & PCs"
    },
    new Employee()
    {
        Id = 2,
        Name = "Cordon Blue",
        Specialty = "Viruses & Malware"
    }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket()
    {
        Id = 1,
        CustomerId = 3,
        EmployeeId = 2,
        Description = "This is a ticket",
        Emergency = false,
        DateCompleted = new DateTime(2023, 07, 31)
    },
    new ServiceTicket()
    {
        Id = 2,
        CustomerId = 3,
        EmployeeId = 1,
        Description = "This is a ticket",
        Emergency = false,
        DateCompleted = new DateTime(2023, 05, 16)
    },
    new ServiceTicket()
    {
        Id = 3,
        CustomerId = 1,
        Description = "This is a ticket",
        Emergency = true
    },
    new ServiceTicket()
    {
        Id = 4,
        CustomerId = 2,
        Description = "This is a ticket",
        Emergency = false
    },
    new ServiceTicket()
    {
        Id = 5,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "This is a ticket",
        Emergency = true,
        DateCompleted = new DateTime(2023, 08, 05)
    }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/servicetickets", () =>
{
    return serviceTickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});



app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
  
    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok( new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = customer  == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        EmployeeId = serviceTicket.EmployeeId,
        Employee = employee == null ? null : new EmployeeDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Specialty = employee.Specialty
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    });
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{

    // Get the customer data to check that the customerid for the service ticket is valid
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    // if the client did not provide a valid customer id, this is a bad request
    if (customer == null)
    {
        return Results.BadRequest();
    }

    // creates a new id (SQL will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);

    // Created returns a 201 status code with a link in the headers to where the new resource can be accessed
    return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency
    });

});



app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);

    ticketToComplete.DateCompleted = DateTime.Today;

});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    ticketToUpdate.CustomerId = serviceTicket.CustomerId;
    ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
    ticketToUpdate.Description = serviceTicket.Description;
    ticketToUpdate.Emergency = serviceTicket.Emergency;
    ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

    return Results.NoContent();
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id  == id);
    if(serviceTicket == null)
    {
        return Results.NotFound();
    }

    serviceTickets.Remove(serviceTicket);

    return Results.NoContent();
});


app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
  
    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        ServiceTickets = tickets.Select(st => new ServiceTicketDTO
        {
            Id = st.Id,
            EmployeeId = st.EmployeeId,
            Description = st. Description,
            Emergency = st.Emergency,
            DateCompleted = st.DateCompleted
        }).ToList()
    });
});


app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    
    });
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty,
          ServiceTickets = tickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    }).ToList()
    });
});

/// new endpoints for all of the extra coding challenge after tuber treats
/// 
/// incomplete emergency tickets
/// 
app.MapGet("/servicetickets/incomplete-emergencies", () =>
{
    List<ServiceTicketDTO> incompleteEmergencyTickets = serviceTickets
        .Where(st => st.DateCompleted == DateTime.MinValue && st.Emergency)
        .Select(st => new ServiceTicketDTO
        {
            Id = st.Id,
            CustomerId = st.CustomerId,
            EmployeeId = st.EmployeeId,
            Description = st.Description,
            Emergency = st.Emergency,
            DateCompleted = st.DateCompleted
        })
        .ToList();

    return Results.Ok(incompleteEmergencyTickets);
});

//unassigned tickets

app.MapGet("/servicetickets/unassigned", () =>
{
    List<ServiceTicketDTO> unassignedTickets = serviceTickets
        .Where(st => st.EmployeeId == null)
        .Select(st => new ServiceTicketDTO
        {
            Id = st.Id,
            CustomerId = st.CustomerId,
            EmployeeId = st.EmployeeId,
            Description = st.Description,
            Emergency = st.Emergency,
            DateCompleted = st.DateCompleted
        })
        .ToList();

    return Results.Ok(unassignedTickets);
});

//inactive customers

app.MapGet("/customers/inactive", () =>
{
    DateTime oneYearAgo = DateTime.Now.AddYears(-1);

    List<CustomerDTO> inactiveCustomers = customers
        .Where(c => !serviceTickets.Any(st => st.CustomerId == c.Id && st.DateCompleted > oneYearAgo))
        .Select(c => new CustomerDTO
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        })
        .ToList();

    return Results.Ok(inactiveCustomers);
});

//available employees-- employees not currently assigned to an incomplete service ticket

app.MapGet("/employees/unassigned", () =>
{
    List<int> assignedEmployeeIds = serviceTickets
        .Where(st => st.DateCompleted == DateTime.MinValue)
        .Select(st => st.EmployeeId ?? -1)
        .Where(id => id != -1)
        .ToList();

    List<EmployeeDTO> unassignedEmployees = employees
        .Where(e => !assignedEmployeeIds.Contains(e.Id))
        .Select(e => new EmployeeDTO
        {
            Id = e.Id,
            Name = e.Name,
            Specialty = e.Specialty
        })
        .ToList();

    return Results.Ok(unassignedEmployees);
});

// return all of the customers for whom a given employee has been assigned to a service ticket

app.MapGet("/customers/by-employee/{employeeId}", (int employeeId) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == employeeId);
    if (employee == null)
    {
        return Results.NotFound("Employee not found");
    }

    List<CustomerDTO> customersServedByEmployee = customers
        .Where(c => serviceTickets.Any(st => st.EmployeeId == employeeId && st.CustomerId == c.Id))
        .Select(c => new CustomerDTO
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address
        })
        .ToList();

    return Results.Ok(customersServedByEmployee);
});

//employee with most service tickets in the month 

app.MapGet("/employees/most-productive-last-month", () =>
{
    DateTime lastMonthStart = DateTime.Now.AddMonths(-1).Date.AddDays(1 - DateTime.Now.AddMonths(-1).Day);
    DateTime lastMonthEnd = DateTime.Now.Date.AddDays(-DateTime.Now.Day);

    List<Employee> employeesWithCompletedTickets = employees
        .Where(e => serviceTickets.Any(st => 
            st.EmployeeId == e.Id && 
            st.DateCompleted >= lastMonthStart && 
            st.DateCompleted <= lastMonthEnd))
        .ToList();

    if (employeesWithCompletedTickets.Count == 0)
    {
        return Results.NotFound("No employees completed tickets last month");
    }

    Employee mostProductiveEmployee = employeesWithCompletedTickets[0];
    int maxCompletedTickets = 0;

    foreach (Employee employee in employeesWithCompletedTickets)
    {
        int completedTickets = serviceTickets.Count(st => 
            st.EmployeeId == employee.Id && 
            st.DateCompleted >= lastMonthStart && 
            st.DateCompleted <= lastMonthEnd);

        if (completedTickets > maxCompletedTickets)
        {
            maxCompletedTickets = completedTickets;
            mostProductiveEmployee = employee;
        }
    }

    EmployeeDTO mostProductiveEmployeeDTO = new EmployeeDTO
    {
        Id = mostProductiveEmployee.Id,
        Name = mostProductiveEmployee.Name,
        Specialty = mostProductiveEmployee.Specialty
    };

    return Results.Ok(mostProductiveEmployeeDTO);
});

//return completed tickets in order of the completion data, oldest first

app.MapGet("/servicetickets/completed", () =>
{
    List<ServiceTicketDTO> completedTickets = serviceTickets
        .Where(st => st.DateCompleted != DateTime.MinValue)
        .OrderBy(st => st.DateCompleted)
        .Select(st => new ServiceTicketDTO
        {
            Id = st.Id,
            CustomerId = st.CustomerId,
            EmployeeId = st.EmployeeId,
            Description = st.Description,
            Emergency = st.Emergency,
            DateCompleted = st.DateCompleted
        })
        .ToList();

    return Results.Ok(completedTickets);
});

//Create an endpoint to return all tickets that are incomplete, in order first by whether they are emergencies, then by whether they are assigned or not (unassigned first).


app.MapGet("/servicetickets/incomplete", () =>
{
    List<ServiceTicketDTO> incompleteTickets = serviceTickets
        .Where(st => st.DateCompleted == DateTime.MinValue)
        .OrderByDescending(st => st.Emergency)
        .ThenBy(st => st.EmployeeId != null)
        .Select(st => new ServiceTicketDTO
        {
            Id = st.Id,
            CustomerId = st.CustomerId,
            EmployeeId = st.EmployeeId,
            Description = st.Description,
            Emergency = st.Emergency,
            DateCompleted = st.DateCompleted
        })
        .ToList();

    return Results.Ok(incompleteTickets);
});

app.Run();

