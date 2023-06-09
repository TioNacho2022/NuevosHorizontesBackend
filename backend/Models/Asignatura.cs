﻿namespace backend.Models
{
    public class Asignatura
    {
        public int Id { get; set; }
        public int Capacidad { get; set; }
        public string Nombre { get; set; }
        public string Horario { get; set; }
        public string Asistencia_id { get; set; }
        public Profesor Profesor { get; set; }

        public class AsignaturaResponse
        {
            public int Id { get; set; }
            public int Capacidad { get; set; }
            public string Nombre { get; set; }
            public int Profesor_id { get; set;}
            

        }

        public class EstudianteResponse
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public Profesor.ResponseEstudiante Profesor { get; set; }


        }

       

    }
}
