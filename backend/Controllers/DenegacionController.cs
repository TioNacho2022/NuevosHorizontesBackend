using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DenegacionController: Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet("{id}")]
        public IActionResult ObtenerDenegacionPorID(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM denegacion WHERE estudiante_id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                       
                        Denegacion.DenegacionResponse denegacion = new Denegacion.DenegacionResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Causa = reader["causa"].ToString(),
                            Estudiante_id = Convert.ToInt32(reader["estudiante_id"]),
                            Fecha = reader["fecha"].ToString()
                        };

                        return Ok(new{
                            Get = true,
                            Denegacion = denegacion
                        });

                    }
                    else
                    {
                        return Ok(new {
                            Get = false,
                            error = "No se encontró el estudiante con el id proporcionado" });
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
