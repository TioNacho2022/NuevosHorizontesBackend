namespace backend.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string P_nombre { get; set; }
        public string S_nombre { get; set; }
        public string Ap_paterno { get; set; }
        public string Ap_materno { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int Edad { get; set; }
        public string Curso_ingreso { get; set; }
        public string Pendiente { get; set; }
        public string Genero { get; set; }


        public class EstudianteResponse
        {
            public int Id { get; set; }
            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
            public string Correo { get; set; }
            public int Edad { get; set; }
            public string Curso_ingreso { get; set; }
            public string Genero { get; set; }
        }

        public class EstudianteGet { 
         
            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
            public int Edad { get; set; }
            public string Curso_ingreso { get; set; }
            public string Genero { get; set; }
        }

    }
}
