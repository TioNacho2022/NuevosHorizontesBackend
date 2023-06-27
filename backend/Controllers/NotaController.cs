using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using static backend.Models.Asistencia;

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

        [HttpPost]
        public IActionResult AgregarEstudiante(Nota nota)
        {
            try
            {


                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryExists = "SELECT COUNT(*) FROM nota WHERE evaluacion_id=:evaluacion_id AND estudiante_id=:estudiante_id AND asignatura_id=:asignatura_id";
                    OracleCommand commandExists = new OracleCommand(queryExists, connection);
                    commandExists.Parameters.Add(":evaluacion_id", OracleDbType.Int32).Value = nota.Evaluacion_id;
                    commandExists.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = nota.Estudiante_id;
                    commandExists.Parameters.Add(":asignatura_id", OracleDbType.Int32).Value = nota.Asignatura_id;
                    int existingCount = Convert.ToInt32(commandExists.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        return Ok(new { add = false });
                    }


                    string query = "INSERT INTO nota (id, evaluacion_id, estudiante_id, asignatura_id, nota) " +
                                    "VALUES (nta_seq.NEXTVAL, :evaluacion_id, :estudiante_id, :asignatura_id, :nota) " +
                                    "RETURNING id INTO :newId";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":evaluacion_id", OracleDbType.Int32).Value = nota.Evaluacion_id;
                    command.Parameters.Add(":estudiante_id", OracleDbType.Varchar2).Value = nota.Estudiante_id;
                    command.Parameters.Add(":asignatura_id", OracleDbType.Varchar2).Value = nota.Asignatura_id;
                    command.Parameters.Add(":nota", OracleDbType.Decimal).Value = nota.nota;
                    




                    OracleParameter newIdParameter = new OracleParameter(":newId", OracleDbType.Int32);
                    newIdParameter.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(newIdParameter);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        OracleDecimal oracleDecimal = (OracleDecimal)newIdParameter.Value;
                        int newId = oracleDecimal.ToInt32();
                        return Ok(new
                        {
                            add = true,
                            id = newId
                        });


                    }
                    else
                    {
                        return BadRequest(new { error = "No se pudo la nota" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }


        }
    }
}
