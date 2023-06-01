namespace backend.Models
{
    public class Sala
    {
        public int Id { get;set; }
        public int Numero { get;set; }

        public int Capacidad { get;set; }

        public class SalaGet
        {
            
            public int Numero { get; set; }
        }

        public class SalaResponse
        {
            public int Id { get; set; }
            public int Numero { get; set; }
        }
    }
}
