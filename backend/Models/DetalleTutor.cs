namespace backend.Models
{
    public class DetalleTutor
    {
        public int Id { get; set; }
        public int EstudianteId { get; set;}
        public int TutorId { get; set; }
        public string Tutor { get; set; }

        public class DetalleTutorGet
        {
           
            public int EstudianteId { get; set; }
            public int TutorId { get; set; }
            public string Tutor { get; set; }
        }

       
    }
    
}
