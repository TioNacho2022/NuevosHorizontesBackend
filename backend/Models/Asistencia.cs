namespace backend.Models
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int Horario_id { get; set; }
        public int Bloque_id { get; set; } 
        public int Estudiante_id { get; set;}
        public int Estado_id { get; set; }
        public string Fecha { get; set; }

        public class Post
        {
            public int Horario_id { get; set; }
            public int Bloque_id { get; set; }
            public Asistencia.Estudiante[] Estudiantes { get; set; } 

        }

        public class Estudiante
        {
            public int Estudiante_id { get; set; }
            public int Estado_id { get; set; }
           

        }
    }
}
