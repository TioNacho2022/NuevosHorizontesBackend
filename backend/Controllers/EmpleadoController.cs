using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet("{id}")]
        public IActionResult ObtenerEmpleadoPorId(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM empleado INNER JOIN rol USING(id) WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;
                    

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Empleado.EmpleadoResponse empleado = new Empleado.EmpleadoResponse
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Rut = reader.GetString(reader.GetOrdinal("rut")),
                            P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                            S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                            Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                            Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                            Correo = reader.GetString(reader.GetOrdinal("correo")),
                            Rol = new Rol
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                            }

                        };

                        return Ok(new
                        {
                            get = true,
                            rol = empleado.Rol.Nombre,
                            usuario = empleado
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
