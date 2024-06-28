using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;

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




app.Run();

