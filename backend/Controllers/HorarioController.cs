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

        [HttpGet("Estudiante/{estudiante_id}")]
        public IActionResult ObtenerHorarioEstudiantePorId(int estudiante_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    int curso_id = 0;

                    string query = "SELECT curso_id FROM estudiante WHERE id=:estudiante_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Estudiante estudiante = new Estudiante()
                        {
                            
                            Curso_id = Convert.ToInt32(reader["curso_id"]),
                            
                        };

                        curso_id = estudiante.Curso_id;
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró el estudiante con el id proporcionado" });
                    }


                    string queryHorario = "SELECT dh.id AS detalle_horario_id,h.id AS horario_id, a.id AS asignatura_id , a.nombre AS asignatura_nombre,c.id AS curso_id,c.nombre AS curso_nombre ,s.id AS sala_id,s.numero AS sala_numero ,p.id AS profesor_id ,p.p_nombre ||' '|| p.ap_paterno AS profesor_nombre , b.id AS bloque_id, b.hora_inicio AS hora_inicio,b.hora_fin AS hora_fin,d.id AS dia_id, d.nombre AS dia_nombre FROM detalle_horario dh INNER JOIN horario h ON dh.horario_id = h.id INNER JOIN asignatura a ON h.asignatura_id = a.id INNER JOIN curso c ON h.curso_id = c.id INNER JOIN sala s ON c.sala_id = s.id INNER JOIN profesor p ON a.profesor_id = p.id INNER JOIN bloque b  ON dh.BLOQUE_ID = b.ID INNER JOIN dia d ON b.dia_id = d.id WHERE h.curso_id = :curso_id";
                    OracleCommand commandHorario = new OracleCommand(queryHorario, connection);
                    commandHorario.Parameters.Add(":curso_id", OracleDbType.Int32).Value = curso_id ;

                    OracleDataReader readerHorario = commandHorario.ExecuteReader();

                    List<DetalleHorario.ResponseEstudiante> detalles_horario = new List<DetalleHorario.ResponseEstudiante>();

                    while (readerHorario.Read())
                    {

                        DetalleHorario.ResponseEstudiante detalle_horario = new DetalleHorario.ResponseEstudiante()
                        { 
                            Id = Convert.ToInt32(readerHorario["detalle_horario_id"]),
                            Horario = new Horario.ResponseEstudiante()
                            {
                                Id = Convert.ToInt32(readerHorario["horario_id"]),
                                Asignatura = new Asignatura.EstudianteResponse()
                                {
                                    Id = Convert.ToInt32(readerHorario["asignatura_id"]),
                                    Nombre = readerHorario["asignatura_nombre"].ToString(),
                                    Profesor = new Profesor.ResponseEstudiante()
                                    {
                                        Id = Convert.ToInt32(readerHorario["profesor_id"]),
                                        Nombre = readerHorario["profesor_nombre"].ToString(),

                                    }
                                },
                                Curso = new Curso()
                                {
                                    Id = Convert.ToInt32(readerHorario["curso_id"]),
                                    Nombre = readerHorario["curso_nombre"].ToString(),
                                    Sala = new Sala()
                                    {
                                        Id = Convert.ToInt32(readerHorario["sala_id"]),
                                        Numero = Convert.ToInt32(readerHorario["sala_numero"])
                                    }
                                }
                            },
                            Bloque = new Bloque.ResponseEstudiante()
                            {
                                Id = Convert.ToInt32(readerHorario["bloque_id"]),
                                HorarioInicio = readerHorario["hora_inicio"].ToString(),
                                HorarioFin = readerHorario["hora_fin"].ToString(),
                                Dia = new Dia.Response()
                                {
                                    Id = Convert.ToInt32(readerHorario["dia_id"]),
                                    Nombre = readerHorario["dia_nombre"].ToString(),
                                }
                            }
                            
                        };

                        detalles_horario.Add(detalle_horario);


                    }

                    return Ok(new { Horario = detalles_horario });

                   
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }

        }

        [HttpGet("Profesor/{profesor_id}")]
        public IActionResult ObtenerHorarioProfesorPorId(int profesor_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();


                    string queryHorario = "SELECT dh.id AS detalle_horario_id,h.id AS horario_id, a.id AS asignatura_id , a.nombre AS asignatura_nombre , c.id AS curso_id, c.nombre AS curso_nombre,s.id AS sala_id,s.numero AS sala_numero,p.id AS profesor_id ,p.p_nombre ||' '|| p.ap_paterno AS profesor_nombre , b.id AS bloque_id, b.hora_inicio AS hora_inicio,b.hora_fin AS hora_fin,d.id AS dia_id, d.nombre AS dia_nombre FROM detalle_horario dh INNER JOIN horario h ON dh.horario_id = h.id INNER JOIN asignatura a ON h.asignatura_id = a.id INNER JOIN curso c ON h.curso_id = c.id INNER JOIN sala s ON c.sala_id = s.id INNER JOIN profesor p ON a.profesor_id = p.id INNER JOIN bloque b  ON dh.BLOQUE_ID = b.ID INNER JOIN dia d ON b.dia_id = d.id WHERE a.profesor_id = :profesor_id";
                    OracleCommand commandHorario = new OracleCommand(queryHorario, connection);
                    commandHorario.Parameters.Add(":profesor_id", OracleDbType.Int32).Value = profesor_id;

                    OracleDataReader readerHorario = commandHorario.ExecuteReader();

                    List<DetalleHorario.ResponseProfesor> detalles_horario = new List<DetalleHorario.ResponseProfesor>();

                    while (readerHorario.Read())
                    {

                        DetalleHorario.ResponseProfesor detalle_horario = new DetalleHorario.ResponseProfesor()
                        {
                            Id = Convert.ToInt32(readerHorario["detalle_horario_id"]),
                            Horario = new Horario.ResponseProfesor()
                            {
                                Id = Convert.ToInt32(readerHorario["horario_id"]),
                                Asignatura = new Asignatura.EstudianteResponse()
                                {
                                    Id = Convert.ToInt32(readerHorario["asignatura_id"]),
                                    Nombre = readerHorario["asignatura_nombre"].ToString(),
                                    Profesor = new Profesor.ResponseEstudiante()
                                    {
                                        Id = Convert.ToInt32(readerHorario["profesor_id"]),
                                        Nombre = readerHorario["profesor_nombre"].ToString(),

                                    }
                                   

                                },
                                Curso = new Curso()
                                {
                                    Id = Convert.ToInt32(readerHorario["curso_id"]),
                                    Nombre = readerHorario["curso_nombre"].ToString(),
                                    Sala = new Sala(){
                                        Id = Convert.ToInt32(readerHorario["sala_id"]),
                                        Numero = Convert.ToInt32(readerHorario["sala_numero"])
                                    }
                                }

                            },
                            Bloque = new Bloque.ResponseEstudiante()
                            {
                                Id = Convert.ToInt32(readerHorario["bloque_id"]),
                                HorarioInicio = readerHorario["hora_inicio"].ToString(),
                                HorarioFin = readerHorario["hora_fin"].ToString(),
                                Dia = new Dia.Response()
                                {
                                    Id = Convert.ToInt32(readerHorario["dia_id"]),
                                    Nombre = readerHorario["dia_nombre"].ToString(),
                                }
                            }

                        };

                        detalles_horario.Add(detalle_horario);


                    }

                    return Ok(new { Horario = detalles_horario });


                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }

        }
    }

    
}

