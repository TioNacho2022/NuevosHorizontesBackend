using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleHorarioController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";
        [HttpGet("{asignatura_id}/{curso_id}")]
        public IActionResult ObtenerTodosLosEstudiantes(int asignatura_id, int curso_id)
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

                    string query = "SELECT * FROM detalle_horario dh INNER JOIN bloque b ON dh.bloque_id=b.id INNER JOIN dia d ON b.dia_id = d.id WHERE dh.horario_id=:horario_id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":horario_id", OracleDbType.Int32).Value = horario.Id;

                    OracleDataReader reader = command.ExecuteReader();

                    List<DetalleHorario.Response> detalles_horario = new List<DetalleHorario.Response>();

                    while (reader.Read())
                    {


                        DetalleHorario.Response detalle_horario = new DetalleHorario.Response()
                        {


                            Id = Convert.ToInt32(reader["id"]),
                            Horario_id = Convert.ToInt32(reader["horario_id"]),
                            Bloque = new Bloque.Response()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                HorarioInicio =  Convert.ToString(reader["hora_inicio"]),
                                HorarioFin = Convert.ToString(reader["hora_fin"]),
                                Dia = new Dia.Response(){
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nombre = Convert.ToString(reader["nombre"]),

                                }
                            }



                        };

                        detalles_horario.Add(detalle_horario);
                    }

                    return Ok(new { 

                        detallesHorario = detalles_horario ,
                        
                        Horario = horario.Id});
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }
    }
}
