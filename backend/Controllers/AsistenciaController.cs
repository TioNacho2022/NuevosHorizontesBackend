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

        
        [HttpGet("{asignatura_id}/{curso_id}/{estudiante_id}")]
        public IActionResult obtenerAsistenciadeestuidante(int asignatura_id, int curso_id,int estudiante_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {

                    connection.Open();

                    string queryHorario = "SELECT * FROM horario  WHERE asignatura_id=:asignatura_id AND curso_id=:curso_id";
                    OracleCommand commandHorario = new OracleCommand(queryHorario, connection);
                    commandHorario.Parameters.Add(":asignatura_id", OracleDbType.Int32).Value = asignatura_id;
                    commandHorario.Parameters.Add(":curso_id", OracleDbType.Int32).Value = curso_id;

                    OracleDataReader reader0 = commandHorario.ExecuteReader();

                    reader0.Read();

                    Horario horario = new Horario()
                    {
                        Id = Convert.ToInt32(reader0["id"]),
                    };



                    string queryCantidad = "SELECT Count(*) AS Asistencias_totales  FROM asistencia a INNER JOIN estado e ON a.estado_id=e.id INNER JOIN bloque b ON a.bloque_id = b.id INNER JOIN dia d ON b.dia_id = d.id WHERE a.horario_id=:horario_id AND a.estudiante_id=:estudiante_id";
                    OracleCommand commandCantidad = new OracleCommand(queryCantidad, connection);
                    commandCantidad.Parameters.Add(":horario_id", OracleDbType.Int32).Value = horario.Id;
                    commandCantidad.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;
                    OracleDataReader readerCantidad = commandCantidad.ExecuteReader();

                    decimal AsistenciasTotales = 0;

                    if (readerCantidad.Read())
                    {
                        AsistenciasTotales = Convert.ToInt32(readerCantidad["Asistencias_totales"]);
                       
                    }

                    string queryCantidadPresente = "SELECT Count(*) AS Asistencias_presente  FROM asistencia a INNER JOIN estado e ON a.estado_id=e.id INNER JOIN bloque b ON a.bloque_id = b.id INNER JOIN dia d ON b.dia_id = d.id WHERE a.horario_id=:horario_id AND a.estudiante_id=:estudiante_id AND a.estado_id=1";
                    OracleCommand commandCantidadPresente = new OracleCommand(queryCantidadPresente, connection);
                    commandCantidadPresente.Parameters.Add(":horario_id", OracleDbType.Int32).Value = horario.Id;
                    commandCantidadPresente.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;
                    OracleDataReader readerCantidadPresente = commandCantidadPresente.ExecuteReader();

                    decimal AsistenciasPresente = 0;

                    if (readerCantidadPresente.Read())
                    {
                        AsistenciasPresente = Convert.ToInt32(readerCantidadPresente["Asistencias_presente"]);

                    }

                    decimal PorcentajeInicial;
                    string Porcentaje = "";

                    if (AsistenciasTotales > 0)
                    {
                        PorcentajeInicial = ((AsistenciasPresente * 100) / AsistenciasTotales);
                        Porcentaje = PorcentajeInicial.ToString("0.0");

                    }

                    







                    string query = "SELECT a.id AS asistencia_id, e.id AS estado_id, e.nombre AS nombre_estado,TO_CHAR(a.fecha, 'DD \"de\" MONTH \"de\" YYYY') AS fecha  ,b.id AS bloque_id ,b.hora_inicio,b.hora_fin, d.id AS dia_id, d.nombre AS dia_nombre FROM asistencia a INNER JOIN estado e ON a.estado_id=e.id INNER JOIN bloque b ON a.bloque_id = b.id INNER JOIN dia d ON b.dia_id = d.id WHERE a.horario_id=:horario_id AND a.estudiante_id=:estudiante_id ORDER BY fecha";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":horario_id", OracleDbType.Int32).Value = horario.Id;
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;
                    OracleDataReader reader = command.ExecuteReader();

                    List<Asistencia.Response> asistencias = new List<Asistencia.Response>();

                    while (reader.Read())
                    {
                        Asistencia.Response asistencia = new Asistencia.Response()
                        {
                            Id = Convert.ToInt32(reader["asistencia_id"]),
                            Bloque = new Bloque.Response()
                            {
                                Id = Convert.ToInt32(reader["bloque_id"]),
                                HorarioInicio = Convert.ToString(reader["hora_inicio"]),
                                HorarioFin = Convert.ToString(reader["hora_fin"]),
                                Dia = new Dia.Response()
                                {
                                    Id = Convert.ToInt32(reader["dia_id"]),
                                    Nombre = Convert.ToString(reader["dia_nombre"])
                                }

                            },
                            Estado = new Estado()
                            {
                                Id = Convert.ToInt32(reader["estado_id"]),
                                Nombre = Convert.ToString(reader["nombre_estado"])
                            },
                            Fecha = Convert.ToString(reader["fecha"]),
                        };

                        asistencias.Add(asistencia);
                    }

                    return Ok(new
                    {
                        Porcentaje = Porcentaje,
                        Asistencias_Presente = AsistenciasPresente,
                        Asistencias_Totales = AsistenciasTotales,
                        Asistencias = asistencias,
                        Horario = horario.Id
                    });
                   


                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }
    }
}
