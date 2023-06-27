namespace backend.Models
{
    public class Nota
    {
        public int Id { get; set; }
        public decimal nota { get;set; }
        public int Evaluacion_id { get; set; }
        public int Estudiante_id { get; set; }
        public int Asignatura_id { get; set; }

        public class Response
        {
            public int Id { get; set; }
            public decimal nota { get; set; }
            public Evaluacion.EvaluacionResponse Evaluacion { get; set; }
            public int Asignatura_id { get; set; }

        }

    }
}
