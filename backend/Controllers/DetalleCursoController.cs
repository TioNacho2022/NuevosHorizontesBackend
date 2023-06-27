using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCursoController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet("id")]
        public IActionResult obtenerAsignaurasCurso(int curso_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT dc.id,dc.curso_id,a.id, a.nombre,a.profesor_id,p.id,p.p_nombre,p.ap_paterno FROM detalle_curso dc INNER JOIN asignatura a ON dc.asignatura_id=a.id INNER JOIN profesor p ON a.profesor_id = p.id WHERE dc.curso_id = :curso_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":curso_id", OracleDbType.Int32).Value = curso_id;
                   

                    OracleDataReader reader = command.ExecuteReader();

                    List<DetalleCurso.Response> detalles_curso = new List<DetalleCurso.Response>();

                    while (reader.Read())
                    {
                        DetalleCurso.Response detalle_curso = new DetalleCurso.Response()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Curso_id = Convert.ToInt32(reader["curso_id"]),
                            Asignatura = new Asignatura()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Nombre = reader["nombre"].ToString(),
                                Profesor = new Profesor()
                                {
                                    P_nombre = reader["p_nombre"].ToString(),
                                    Ap_paterno = reader["ap_paterno"].ToString(),
                                }

                            }
                        };

                        detalles_curso.Add(detalle_curso);
                    }   

                    return Ok(new{ Detalles_curso = detalles_curso});
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet("asignatura_id")]
        public IActionResult obtenerCursoasignatura(int asignatura_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT dc.id AS detalle_curso_id,c.id AS curso_id,dc.asignatura_id,c.nombre FROM detalle_curso dc INNER JOIN curso c ON dc.curso_id=c.id WHERE dc.asignatura_id=:asignatura_id ";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":asignatura_id", OracleDbType.Int32).Value = asignatura_id;


                    OracleDataReader reader = command.ExecuteReader();

                    List<DetalleCurso.ResponseCurso> detalles_curso = new List<DetalleCurso.ResponseCurso>();

                    while (reader.Read())
                    {
                        DetalleCurso.ResponseCurso detalle_curso = new DetalleCurso.ResponseCurso()
                        {
                            Id = Convert.ToInt32(reader["detalle_curso_id"]),
                            Curso = new Curso()
                            {
                                Id = Convert.ToInt32(reader["curso_id"]),
                                Nombre = Convert.ToString(reader["nombre"]),
                            },
                            Asignatura_id = Convert.ToInt32(reader["asignatura_id"]),


                        };

                        detalles_curso.Add(detalle_curso);
                    }

                    return Ok(new { Detalles_curso = detalles_curso });
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }


    }
}
