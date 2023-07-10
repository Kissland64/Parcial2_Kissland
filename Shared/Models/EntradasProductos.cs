using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EntradasProducto{

    [Key]
    public int EntradaId {get; set;}
    public DateTime Fecha {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public string Concepto {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public string PesoTotal {get; set;}
    public int ProductoId {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]
    public string CantidadProducida {get; set;}
    [Required(ErrorMessage = "Este campo es necesario")]

    [ForeignKey("DetalleId")]
    public ICollection<ProductoDetalle> ProductoDetalles { get; set; } = new List<ProductoDetalle>();
}