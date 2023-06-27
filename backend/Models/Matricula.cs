using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace backend.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public int Valor { get; set; }
        public Estudiante.EstudianteResponse Estudiante { get;set; }
        public string Confirmada { get; set; }
        public string Pagada { get; set; }
        public class Pago
        {
            public int Id { get; set; }
            public int Valor { get; set; }
            public int Estudiante_id { get; set; }
        }

        public class Respuesta
        {
            public int Estudiante_id { get; set; }
            public string Token_ws { get; set; }
        }
    }
}

