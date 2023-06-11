using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignaturaController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";
        [HttpGet("{profesor_id}")]
        public IActionResult ObtenerEmpleadoPorId(int profesor_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM asignatura  WHERE profesor_id = :profesor_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = profesor_id;


                    OracleDataReader reader = command.ExecuteReader();

                    
                    List<Asignatura.AsignaturaResponse> asignaturas = new List<Asignatura.AsignaturaResponse>();

                    while (reader.Read())
                    {
                        Asignatura.AsignaturaResponse asignatura = new Asignatura.AsignaturaResponse
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Capacidad = reader.GetInt32(reader.GetOrdinal("capacidad")),
                            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            Profesor_id = reader.GetInt32(reader.GetOrdinal("profesor_id")),


                        };

                        asignaturas.Add(asignatura);
                    }

                    return Ok(new { Asignaturas = asignaturas });

               
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }


        }
    }
}
