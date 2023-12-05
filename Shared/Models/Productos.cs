using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Productos{

    [Key]
    public int ProductoId {get; set;}

    [Required(ErrorMessage = "Este campo es necesario")]
    public string Descripcion {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public int Tipo {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public double Existencia {get; set;}
    public decimal Peso {get; set;}
}