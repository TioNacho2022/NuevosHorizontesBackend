using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CursoController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet]
        public IActionResult obtenerTodosLosCursos()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT curso.id, curso.nombre, curso.sala_id, sala.numero, sala.capacidad FROM curso JOIN sala ON curso.sala_id = sala.id";

                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Curso> cursos = new List<Curso>();

                    while (reader.Read())
                    {
                        Curso curso = new Curso()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Nombre = reader["nombre"].ToString(),
                            Sala = new Sala()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Numero = Convert.ToInt32(reader["numero"]),
                                Capacidad = Convert.ToInt32(reader["capacidad"])
                            }
                        };

                        cursos.Add(curso);
                    }   

                    return Ok(new{ Cursos = cursos});
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        
    }
}
