namespace backend.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Sala_id { get; set; }
        public Sala Sala { get; set; }
        
    }
}
