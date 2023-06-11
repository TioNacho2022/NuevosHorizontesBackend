using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotaController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet]
        public IActionResult obtenerNotasEstudiante(int asignatura_id, int estudiante_id)
        {
            
            try
            {
                

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM nota n INNER JOIN evaluacion e ON n.evaluacion_id = e.id WHERE n.estudiante_id = :estudiante_id AND n.asignatura_id =:asignatura_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;
                    command.Parameters.Add(":asignatura_id", OracleDbType.Int32).Value = asignatura_id;


                    OracleDataReader reader = command.ExecuteReader();

                    List<Nota.Response> notas = new List<Nota.Response>();

                    decimal nota1 = 0 ;
                    decimal notas1 = 0;
                    decimal promedio = 0;
                    int contador = 0;

                    while (reader.Read())
                    {
                        Nota.Response nota = new Nota.Response()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            nota = Convert.ToDecimal(reader["nota"]),
                            Evaluacion = new Evaluacion.EvaluacionResponse()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Nombre = reader["nombre"].ToString(),
                               

                            },
                            
                        };

                        notas.Add(nota);

                        contador += 1;

                        nota1 = Convert.ToDecimal(nota.nota);

                        notas1 += nota1;

                        promedio = notas1 / contador;

                        promedio = Math.Round(promedio, 1);







                    }

                    return Ok(new
                    {
                        Notas = notas,
                        Promedio = promedio,
                       
                       
                    }) ;
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }
    }
}
