namespace backend.Models
{
    public class DetalleHorario
    {
        public int Id { get; set; }
        public int Horario_id { get; set; }
        public int Bloque_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public int Horario_id { get; set; }
            public Bloque.Response Bloque { get; set; }
        }
    }
}
