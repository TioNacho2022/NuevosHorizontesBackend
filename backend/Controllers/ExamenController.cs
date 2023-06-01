
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamenController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";
        [HttpPost]
        public IActionResult agregarExamen(Examen.ExamenGet examen )
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    Console.Write(examen.Estudiante_id);

                    string queryexists = "select count(*) from examen where estudiante_id = :id";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":id", OracleDbType.Varchar2).Value = examen.Estudiante_id;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 1)
                    {

                        return Ok(new
                        {
                            add = false,
                            
                        });
                    }

                    string query = "insert into examen (id, correcta_pregunta, incorrecta_pregunta, total_pregunta, nota, diagnostico_asignatura_id, estudiante_id) " +
                                   "values (exm_seq.nextval, :correcta_pregunta, :incorrecta_pregunta, : total_pregunta, :nota, :diagnostico_asignatura_id, :estudiante_id) " +
                                   "returning id into :newid";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":correcta_pregunta", OracleDbType.Int32).Value = examen.Correcta_pregunta;
                    command.Parameters.Add(":incorrecta_pregunta", OracleDbType.Int32).Value = examen.Incorrecta_pregunta;
                    command.Parameters.Add(":total_pregunta", OracleDbType.Int32).Value = examen.Total_pregunta;
                    command.Parameters.Add(":nota", OracleDbType.Decimal).Value = examen.Nota;
                    command.Parameters.Add(":diagnostico_asignatura_id", OracleDbType.Int32).Value = examen.Diagnostico_asignatura_id;
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = examen.Estudiante_id;

                    OracleParameter newidparameter = new OracleParameter(":newid", OracleDbType.Int32);
                    newidparameter.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(newidparameter);

                    int rowsaffected = command.ExecuteNonQuery();

                    if (rowsaffected > 0)
                    {
                        OracleDecimal oracledecimal = (OracleDecimal)newidparameter.Value;
                        int newid = oracledecimal.ToInt32();
                        return Ok(new
                        {
                            add = true,
                            id = newid
                        });
                    }
                    else
                    {
                        return BadRequest(new { error = "no se pudo agregar el estudiante" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "error al conectar a la base de datos de oracle: " + ex.Message });
            }


        }

        [HttpGet("{id}")]
        public IActionResult obtenerExamenes(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM examen WHERE estudiante_id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    List<Examen.ExamenResponse> examenes = new List<Examen.ExamenResponse>();

                    while (reader.Read())
                    {
                        Examen.ExamenResponse examen = new Examen.ExamenResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Correcta_pregunta = Convert.ToInt32(reader["correcta_pregunta"]),
                            Incorrecta_pregunta = Convert.ToInt32(reader["incorrecta_pregunta"]),
                            Total_pregunta = Convert.ToInt32(reader["total_pregunta"]),
                            Nota = Convert.ToDecimal(reader["nota"]),
                            Diagnostico_asignatura_id = Convert.ToInt32(reader["diagnostico_asignatura_id"]),
                            Estudiante_id = Convert.ToInt32(reader["estudiante_id"]),
                        };

                        examenes.Add(examen);
                    }

                    return Ok(examenes);
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }


    }
}
