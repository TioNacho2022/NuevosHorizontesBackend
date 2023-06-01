namespace backend.Models
{
    public class Denegacion
    {
        public int Id { get; set; }
        public string Causa { get; set; }
        public int Estudiante_id { get; set; }
        public string Fecha { get; set; }

        public class DenegacionGet
        {
            public string Causa { get; set; }
            public int Estudiante_id { get; set; }
            
        }

        public class DenegacionResponse
        {
            public int Id { get; set; }
            public string Causa { get; set; }
            public int Estudiante_id { get; set; }
            public string Fecha { get; set; }
        }
    }


}
