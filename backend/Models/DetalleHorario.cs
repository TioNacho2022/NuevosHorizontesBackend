namespace backend.Models
{
    public class DetalleHorario
    {
        public int Id { get; set; }
        public int Horario_id { get; set; }
        public int Bloque_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public int Horario_id { get; set; }
            public Bloque.Response Bloque { get; set; }
        }

        public class ResponseEstudiante
        {
            public int Id { get; set; }
            public Horario.ResponseEstudiante Horario { get; set; }
            public Bloque.ResponseEstudiante Bloque { get; set; }
        }

        public class ResponseProfesor
        {
            public int Id { get; set; }
            public Horario.ResponseProfesor Horario { get; set; }
            public Bloque.ResponseEstudiante Bloque { get; set; }
        }
    }
}
