namespace backend.Models
{
    public class DetalleApoderado
    {
        public int Id { get; set; }
        public int EstudianteId { get; set;}
        public int ApoderadoId { get; set; }
        public string Tutor { get; set; }

        public class DetalleApoderadoGet
        {
           
            public int EstudianteId { get; set; }
            public int ApoderadoId { get; set; }
            public string Tutor { get; set; }
        }

       
    }
    
}
