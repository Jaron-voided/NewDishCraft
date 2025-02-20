namespace Application.Ingredients;

public class NutritionalInfoDto
{
    public Guid IngredientId { get; set; }
    public double Calories { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
}
