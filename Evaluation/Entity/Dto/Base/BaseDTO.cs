using System;

namespace Entity.Dtos.Base
{
    /// <summary>
    /// DTO base único para toda la aplicación
    /// Contiene las propiedades fundamentales que toda entidad necesita
    /// Integrado con OAuth 2.0 para auditoría automática
    /// </summary>
    public abstract class BaseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
    }
}
