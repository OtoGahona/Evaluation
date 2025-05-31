using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto.ProductoDTO
{
    public class UpdateStockDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        public int Cantidad { get; set; }
    }
}
