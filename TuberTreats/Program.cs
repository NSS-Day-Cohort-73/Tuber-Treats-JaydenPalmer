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
        CustomerId = 5,
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
        ToppingId = 5,
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
        //getting the tuberToppings to filter the toppings
        List<TuberTopping> tuberToppingsForDetails = tuberToppings
            .Where(tt => tt.TuberOrderId == id)
            .ToList();
        //using the filtered tuberToppings to grab the correct toppings
        List<Topping> toppingsForDetails = toppings
            .Where(t => tuberToppingsForDetails.Any(tt => tt.ToppingId == t.Id))
            .ToList();

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
            new TuberOrderDTO
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
                Toppings =
                    tuberToppingsForDetails != null
                        ? toppingsForDetails
                            .Select(t => new ToppingDTO { Id = t.Id, Name = t.Name })
                            .ToList()
                        : null,
            }
        );
    }
);

//creating a new order :D
app.MapPost(
    "/tuberorders",
    (TuberOrder newOrder) =>
    {
        TuberOrder orderToAdd = new TuberOrder
        {
            Id = orders.Max(o => o.Id) + 1,
            CustomerId = newOrder.CustomerId,
            OrderPlacedOnDate = DateTime.Now,
            TuberDriverId = newOrder.TuberDriverId != null ? newOrder.TuberDriverId : null,
            DeliveredOnDate = null,
        };

        orders.Add(orderToAdd);
        return Results.Created($"/orders/{orderToAdd.Id}", orderToAdd);
    }
);

//complete an order !
app.MapPost(
    "/tuberorders/{id}/complete",
    (int id) =>
    {
        TuberOrder orderToBeCompleted = orders.FirstOrDefault(o => o.Id == id);

        if (orderToBeCompleted == null || orderToBeCompleted.DeliveredOnDate != null)
        {
            Results.BadRequest("that order either does not exist or has already been delivered");
        }

        orderToBeCompleted.DeliveredOnDate = DateTime.Now;

        return Results.NoContent();
    }
);

//get all the toppings
app.MapGet(
    "/toppings",
    () =>
    {
        return toppings.Select(t => new ToppingDTO { Id = t.Id, Name = t.Name }).ToList();
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

//create a new tuber topping/add a topping to an order
app.MapPost(
    "/tubertoppings",
    (TuberToppingDTO newTuberTopping) =>
    {
        TuberOrder order = orders.FirstOrDefault(o => o.Id == newTuberTopping.TuberOrderId);
        Topping topping = toppings.FirstOrDefault(t => t.Id == newTuberTopping.ToppingId);

        if (order == null || topping == null)
        {
            return Results.NotFound();
        }

        bool toppingIsAlreadyOnOrder = tuberToppings.Any(tt =>
            tt.TuberOrderId == newTuberTopping.TuberOrderId
            && tt.ToppingId == newTuberTopping.ToppingId
        );

        if (toppingIsAlreadyOnOrder)
        {
            return Results.BadRequest("This topping is already on the order");
        }

        TuberTopping tuberToppingToAdd = new TuberTopping
        {
            Id = tuberToppings.Max(tt => tt.Id) + 1,
            TuberOrderId = newTuberTopping.TuberOrderId,
            ToppingId = newTuberTopping.ToppingId,
        };

        tuberToppings.Add(tuberToppingToAdd);

        return Results.Created($"/tubertoppings/{tuberToppingToAdd.Id}", tuberToppingToAdd);
    }
);

//delete a tubertopping
app.MapDelete(
    "/tubertoppings/{id}",
    (int id) =>
    {
        TuberTopping tuberToppingForDelete = tuberToppings.FirstOrDefault(tt => tt.Id == id);

        if (tuberToppingForDelete == null)
        {
            Results.NotFound();
        }

        tuberToppings.Remove(tuberToppingForDelete);
        return Results.NoContent();
    }
);

//get all the drivers/employees
app.MapGet(
    "/tuberdrivers",
    () =>
    {
        return drivers.Select(d => new TuberDriverDTO { Id = d.Id, Name = d.Name }).ToList();
    }
);

app.MapGet(
    "/tuberdrivers/{id}",
    (int id) =>
    {
        TuberDriver tuberdriverForDetails = drivers.FirstOrDefault(d => d.Id == id);

        if (tuberdriverForDetails == null)
        {
            Results.NotFound();
        }

        tuberdriverForDetails.TuberDeliveries = orders.Where(o => o.TuberDriverId == id).ToList();

        return Results.Ok(tuberdriverForDetails);
    }
);

//get all the customers
app.MapGet(
    "/customers",
    () =>
    {
        return customers.Select(c => new Customer
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
        });
    }
);

//get a singular customer
app.MapGet(
    "/customers/{id}",
    (int id) =>
    {
        Customer customerForDetails = customers.FirstOrDefault(c => c.Id == id);

        if (customerForDetails == null)
        {
            Results.NotFound();
        }

        customerForDetails.TuberOrders = orders.Where(o => o.CustomerId == id).ToList();

        return Results.Ok(customerForDetails);
    }
);

//add a customer
app.MapPost(
    "/customers",
    (Customer newCustomer) =>
    {
        Customer customerToAdd = new Customer
        {
            Id = customers.Max(c => c.Id) + 1,
            Name = newCustomer.Name,
            Address = newCustomer.Address,
        };

        customers.Add(customerToAdd);

        return Results.Created($"/customers/{customerToAdd.Id}", customerToAdd);
    }
);

//delete customer
app.MapDelete(
    "/customers/{id}",
    (int id) =>
    {
        Customer customerForDelete = customers.FirstOrDefault(c => c.Id == id);

        if (customerForDelete == null)
        {
            Results.NotFound();
        }

        customers.Remove(customerForDelete);
        return Results.NoContent();
    }
);

app.Run();

//don't touch or move this!
public partial class Program { }
