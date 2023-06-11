using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorarioController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet("{id}")]
        public IActionResult ObtenerEstudiantePorId(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT e.id, e.rut, e.p_nombre, e.s_nombre, e.ap_paterno, e.ap_materno, e.correo, e.edad, e.curso_ingreso, e.genero, e.curso_id, c.id AS curso_id, c.nombre AS curso_nombre, e.continuidad FROM estudiante e INNER JOIN curso c ON e.curso_id = c.id WHERE e.id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        object cursoIdValue = reader["curso_id"];
                        int cursoId;

                        object continuidadValue = reader["continuidad"];
                        string continuidad;

                        if (cursoIdValue != DBNull.Value)
                        {
                            cursoId = Convert.ToInt32(cursoIdValue);
                        }
                        else
                        {

                            cursoId = 0;
                        };

                        if (continuidadValue != DBNull.Value)
                        {
                            continuidad = reader["continuidad"].ToString();
                        }
                        else
                        {

                            continuidad = "pendiente";
                        };

                        Estudiante.EstudianteResponse estudiante = new Estudiante.EstudianteResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Rut = reader["rut"].ToString(),
                            P_nombre = reader["p_nombre"].ToString(),
                            S_nombre = reader["s_nombre"].ToString(),
                            Ap_paterno = reader["ap_paterno"].ToString(),
                            Ap_materno = reader["ap_materno"].ToString(),
                            Correo = reader["correo"].ToString(),
                            Edad = Convert.ToInt32(reader["edad"]),
                            Curso_ingreso = reader["curso_ingreso"].ToString(),
                            Genero = reader["genero"].ToString(),
                            Curso_id = cursoId,
                            Curso = new Curso()
                            {
                                Id = Convert.ToInt32(reader["curso_id"]),
                                Nombre = reader["curso_nombre"].ToString(),
                            },
                            Continuidad = continuidad
                        };

                        return Ok(new
                        {
                            get = true,
                            rol = "Estudiante",
                            usuario = estudiante,

                        }
                           );
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró el estudiante con el id proporcionado" });
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
