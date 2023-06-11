namespace backend.Models
{
    public class Bloque
    {
        public int Id { get; set; }
        public string HorarioInicio { get; set; }
        public string HorarioFin { get; set; }
        public int Dia_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public string HorarioInicio { get; set; }
            public string HorarioFin { get; set; }
            public Dia.Response Dia{ get; set;}
        }
    }
}
