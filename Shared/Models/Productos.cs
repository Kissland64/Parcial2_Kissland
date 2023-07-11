using System.ComponentModel.DataAnnotations;

public class Productos{

    [Key]
    public int ProductoId {get; set;}

    [Required(ErrorMessage = "Este campo es necesario")]
    public string Descripcion {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public string Tipo {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public string Existencia {get; set;}
}