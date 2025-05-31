using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto.ProductoDTO
{
    public class ProductoPriceRangeDto
    {
        [Required(ErrorMessage = "El precio mínimo es requerido.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser mayor o igual a 0.")]
        public decimal PrecioMinimo { get; set; }

        [Required(ErrorMessage = "El precio máximo es requerido.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser mayor o igual a 0.")]
        public decimal PrecioMaximo { get; set; }
    }
}
