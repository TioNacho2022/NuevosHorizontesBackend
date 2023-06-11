namespace backend.Models
{
    public class DetalleCurso
    {
        public int Id { get; set; }
        public int Curso_id { get; set; }
        public int Asignatura_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public int Curso_id { get; set; }
            public Asignatura Asignatura { get; set; }

        }

        public class ResponseCurso
        {
            public int Id { get; set; }
            public Curso Curso { get; set; }
            public int Asignatura_id { get; set; }

        }

    }
}
