namespace Dealership.DTO;
public class VehicleDto
{
    public int Id { get; set; }
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public decimal Price { get; set; }

    public bool InStock { get; set; }
}

public class VehicleCreateDto
{
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int Year { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
}