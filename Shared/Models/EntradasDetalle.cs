using System.ComponentModel.DataAnnotations;
public class EntradasDetalle
{
    [Key]
    public int DetalleId {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public int EntradaId {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public int ProductoId {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public double CantidadUtilizada {get; set;}
}