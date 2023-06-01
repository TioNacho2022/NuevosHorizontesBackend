namespace backend.Models
{
    public class Examen
    {
        public int Id { get; set; }
        public int Correctas_pregunta { get; set; }
        public int Incorrecta_pregunta { get; set; }
        public int Total_pregunta { get; set; }
        public int Nota { get; set; }
        public int Diagnostico_asignatura { get; set; }
        public int Diagnostico_asignatura_id { get; set; }
        public int Estudiante_id { get; set; }

        public class ExamenGet
        {
           
            public int Correcta_pregunta { get; set; }
            public int Incorrecta_pregunta { get; set; }
            public int Total_pregunta { get; set; }
            public decimal Nota { get; set; }
            public int Diagnostico_asignatura_id { get; set; }
            public int Estudiante_id { get; set; }

        }

        public class ExamenResponse
        {
            public int Id { get; set; }
            public int Correcta_pregunta { get; set; }
            public int Incorrecta_pregunta { get; set; }
            public int Total_pregunta { get; set; }
            public decimal Nota { get; set; }
            public int Diagnostico_asignatura_id { get; set; }
            public int Estudiante_id { get; set; }

        }
    }
}
