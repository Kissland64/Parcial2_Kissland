using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class EntradasDetalle
{
    public int DetalleId {get; set;}
    public int EntradaId {get; set;}
    public int ProductoId {get; set;}
    public string CantidadUtilizada {get; set;}
}