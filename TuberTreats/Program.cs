using TuberTreats.Models;
using TuberTreats.Models.DTOs;

List<TuberDriver> drivers = new List<TuberDriver>
{
    new TuberDriver { Id = 1, Name = "John Doe" },
    new TuberDriver { Id = 2, Name = "Jane Smith" },
    new TuberDriver { Id = 3, Name = "Mike Johnson" },
};

List<Customer> customers = new List<Customer>
{
    new Customer
    {
        Id = 1,
        Name = "Alice Brown",
        Address = "123 Main St",
    },
    new Customer
    {
        Id = 2,
        Name = "Bob Wilson",
        Address = "456 Oak Ave",
    },
    new Customer
    {
        Id = 3,
        Name = "Carol White",
        Address = "789 Pine Rd",
    },
    new Customer
    {
        Id = 4,
        Name = "David Black",
        Address = "321 Elm St",
    },
    new Customer
    {
        Id = 5,
        Name = "Eve Green",
        Address = "654 Maple Dr",
    },
};

List<Topping> toppings = new List<Topping>
{
    new Topping { Id = 1, Name = "Cheese" },
    new Topping { Id = 2, Name = "Bacon" },
    new Topping { Id = 3, Name = "Sour Cream" },
    new Topping { Id = 4, Name = "Chives" },
    new Topping { Id = 5, Name = "Butter" },
};

List<TuberOrder> orders = new List<TuberOrder>
{
    new TuberOrder
    {
        Id = 1,
        CustomerId = 1,
        TuberDriverId = 1,
        OrderPlacedOnDate = new DateTime(2024, 1, 1),
        DeliveredOnDate = new DateTime(2024, 1, 1),
    },
    new TuberOrder
    {
        Id = 2,
        CustomerId = 2,
        TuberDriverId = 2,
        OrderPlacedOnDate = new DateTime(2024, 1, 2),
        DeliveredOnDate = new DateTime(2024, 1, 2),
    },
    new TuberOrder
    {
        Id = 3,
        CustomerId = 3,
        TuberDriverId = null,
        OrderPlacedOnDate = new DateTime(2024, 1, 3),
        DeliveredOnDate = null,
    },
};

List<TuberTopping> tuberToppings = new List<TuberTopping>
{
    new TuberTopping
    {
        Id = 1,
        TuberOrderId = 1,
        ToppingId = 1,
    },
    new TuberTopping
    {
        Id = 2,
        TuberOrderId = 1,
        ToppingId = 2,
    },
    new TuberTopping
    {
        Id = 3,
        TuberOrderId = 2,
        ToppingId = 3,
    },
    new TuberTopping
    {
        Id = 4,
        TuberOrderId = 3,
        ToppingId = 4,
    },
    new TuberTopping
    {
        Id = 5,
        TuberOrderId = 3,
        ToppingId = 5,
    },
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

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

app.UseAuthorization();

//add endpoints here

//get all the tuber orders
app.MapGet(
    "/tuberorders",
    () =>
    {
        return orders.Select(order => new TuberOrderDTO
        {
            Id = order.Id,
            OrderPlacedOnDate = order.OrderPlacedOnDate,
            CustomerId = order.CustomerId,
            TuberDriverId = order.TuberDriverId,
            DeliveredOnDate = order.DeliveredOnDate,
        });
    }
);

//get specific tuber order with customer and driver data
app.MapGet(
    "tuberorders/{id}",
    (int id) =>
    {
        TuberOrder orderForDetails = orders.FirstOrDefault(order => order.Id == id);
        Customer customerForDetails = customers.FirstOrDefault(customer =>
            orderForDetails.CustomerId == customer.Id
        );

        TuberDriver driverForDetails = null;
        if (orderForDetails.TuberDriverId != null)
        {
            driverForDetails = drivers.FirstOrDefault(driver =>
                driver.Id == orderForDetails.TuberDriverId
            );
        }

        if (orderForDetails == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(
            new TuberOrderDetailsDTO
            {
                Id = orderForDetails.Id,
                OrderPlacedOnDate = orderForDetails.OrderPlacedOnDate,
                CustomerId = orderForDetails.CustomerId,
                TuberDriverId =
                    orderForDetails.TuberDriverId != null ? orderForDetails.TuberDriverId : null,
                DeliveredOnDate = orderForDetails.DeliveredOnDate,
                Customer = new CustomerDTO
                {
                    Id = customerForDetails.Id,
                    Address = customerForDetails.Address,
                    Name = customerForDetails.Name,
                },
                TuberDriver =
                    driverForDetails != null
                        ? new TuberDriverDTO
                        {
                            Id = driverForDetails.Id,
                            Name = driverForDetails.Name,
                        }
                        : null,
            }
        );
    }
);

//get all the toppings
app.MapGet(
    "/toppings",
    () =>
    {
        return toppings.Select(t => new ToppingDTO { Id = t.Id, Name = t.Name });
    }
);

//get a specific topping
app.MapGet(
    "/toppings/{id}",
    (int id) =>
    {
        Topping toppingForDetails = toppings.FirstOrDefault(t => t.Id == id);

        if (toppingForDetails == null)
        {
            Results.NotFound();
        }

        return Results.Ok(
            new ToppingDTO { Id = toppingForDetails.Id, Name = toppingForDetails.Name }
        );
    }
);

//get all the tuber toppings
app.MapGet(
    "/tubertoppings",
    () =>
    {
        return tuberToppings.Select(tt => new TuberToppingDTO
        {
            Id = tt.Id,
            TuberOrderId = tt.TuberOrderId,
            ToppingId = tt.ToppingId,
        });
    }
);

app.Run();

//don't touch or move this!
public partial class Program { }
