using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesorController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";
        
        [HttpGet("{id}")]
        public IActionResult ObtenerProfesorPorId(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM profesor  WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;


                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Profesor.Response profesor = new Profesor.Response
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Rut = reader.GetString(reader.GetOrdinal("rut")),
                            P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                            S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                            Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                            Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                            Correo = reader.GetString(reader.GetOrdinal("correo")),
                           

                        };

                        return Ok(new
                        {
                            get = true,
                            rol = "Profesor",
                            usuario = profesor
                        });
                    }
                    else
                    {
                        return Ok(new { get = false });
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
