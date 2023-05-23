namespace backend.Models
{
    public class Tutor
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string P_nombre { get; set; }
        public string S_nombre { get; set; }
        public string Ap_paterno { get; set; }
        public string Ap_materno { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int Celular { get; set; }
      
       


        public class TutorResponse
        {
            public int Id { get; set; }
            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
            public string Correo { get; set; }
            public int Celular { get; set; }

        }

        public class TutorGet
        {

            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
            public int Celular { get; set; }
        }

    }
}
