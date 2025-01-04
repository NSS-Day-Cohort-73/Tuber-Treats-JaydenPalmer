namespace TuberTreats.Models;

public class TuberTopping
{
    public int Id { get; set; }
    public int TuberOrderId { get; set; }
    public int ToppingId { get; set; }
    public TuberOrder TuberOrder { get; set; }
    public Topping Topping { get; set; }
}
