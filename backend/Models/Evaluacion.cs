namespace backend.Models
{
    public class Evaluacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Porcentaje { get; set; }

        public class EvaluacionResponse
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public decimal Porcentaje { get; set; }
        }
    }
}
