namespace backend.Models
{
    public class Horario
    {
        public int Id { get; set; }
        public int Asignatura_id { get; set; }
        public int Curso_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public int Asignatura_id { get; set; }
            public int Curso_id { get; set; }
        }

        public class ResponseEstudiante
        {
            public int Id { get; set; }
            public Asignatura.EstudianteResponse Asignatura { get; set; }
            public Curso Curso { get; set; }




        }

        public class ResponseProfesor
        {
            public int Id { get; set; }
            public Asignatura.EstudianteResponse Asignatura { get; set; }
            public Curso Curso  { get; set; }




        }


    }
}
