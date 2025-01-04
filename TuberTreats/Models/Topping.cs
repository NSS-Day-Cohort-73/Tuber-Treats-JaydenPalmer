namespace TuberTreats.Models;

public class Topping
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<TuberTopping> TuberToppings { get; set; }
}
