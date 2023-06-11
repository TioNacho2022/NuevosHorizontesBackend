using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using static backend.Models.Asistencia;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult AgregarAsistencia(Asistencia.Post asistencias)
        {
            try
            {
                string fechaActual = DateTime.Now.ToString("dd/MM/yyyy");

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();



                    string queryExists = "SELECT COUNT(*) FROM asistencia WHERE bloque_id = :bloque_id AND TO_CHAR(fecha, 'DD-MM-YYYY') = :fecha";
                    OracleCommand commandExists = new OracleCommand(queryExists, connection);
                    commandExists.Parameters.Add(":bloque_id", OracleDbType.Int32).Value = asistencias.Bloque_id;
                    commandExists.Parameters.Add(":fecha", OracleDbType.Varchar2).Value = fechaActual;
                    int existingCount = Convert.ToInt32(commandExists.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        return Ok(new { add = false });
                    }



                    foreach (var estudiante in asistencias.Estudiantes)
                    {

                        string query = "INSERT INTO asistencia (id,horario_id, bloque_id, estudiante_id,estado_id, fecha) " +
                                 "VALUES (ast_seq.NEXTVAL,:horario_id,:bloque_id,:estudiante_id,:estado_id,:fecha) ";
                                 

                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":horario_id", OracleDbType.Int32).Value = asistencias.Horario_id;
                        command.Parameters.Add(":bloque_id", OracleDbType.Int32).Value = asistencias.Bloque_id;
                        command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante.Estudiante_id;
                        command.Parameters.Add(":estado_id", OracleDbType.Int32).Value = estudiante.Estado_id;
                        command.Parameters.Add(":fecha", OracleDbType.Varchar2).Value = fechaActual;

                        command.ExecuteNonQuery();

                       

                        

                    }return Ok(new {add = true});

                    
                    




                    
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }


        }
    }
}
