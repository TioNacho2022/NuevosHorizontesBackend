namespace backend.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string P_nombre { get; set; }
        public string S_nombre { get; set; }
        public string Ap_paterno { get; set; }
        public string Ap_materno { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int  Id_rol { get; set; }
      

        public class EmpleadoResponse
        {
            public int Id { get; set; }
            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
            public string Correo { get; set; }
            public  Rol Rol{get;set;}
        }

        public class EmpleadoGet { 
         
            public string Rut { get; set; }
            public string P_nombre { get; set; }
            public string S_nombre { get; set; }
            public string Ap_paterno { get; set; }
            public string Ap_materno { get; set; }
           
        }

    }
}
