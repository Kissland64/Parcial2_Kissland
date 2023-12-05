using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Entradas{

    [Key]
    public int EntradaId {get; set;}

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime Fecha {get; set;} = DateTime.Now;

    [Required(ErrorMessage = "Este campo es necesario")]
    public string Concepto {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]

    public decimal PesoTotal {get; set;}
    
    public int ProductoId {get; set;}

    [Required(ErrorMessage = "Este campo es necesario")]
    public double CantidadProducida {get; set;}

    [ForeignKey("EntradaId")]
    public ICollection<EntradasDetalle> EntradasDetalles { get; set; } = new List<EntradasDetalle>();
}