using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto.ProductoDTO
{
    public class DeleteLogicProductoDto
    {
        [Required(ErrorMessage = "El ID del producto es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        public bool Status { get; set; }
    }
}
