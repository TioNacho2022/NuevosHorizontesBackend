using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleTutorController : Controller
    {
       
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult AgregarDetalleTutor(DetalleTutor.DetalleTutorGet detalleTutor)
        {
            try
            {


                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();


                    string query = "INSERT INTO detalle_tutor (id, estudiante_id, tutor_id, tutor) " +
                                   "VALUES (est_seq.NEXTVAL, :estudiante_id, :tutor_id, :tutor) ";
                                  

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = detalleTutor.EstudianteId;
                    command.Parameters.Add(":tutor_id", OracleDbType.Int32).Value = detalleTutor.TutorId;
                    command.Parameters.Add(":tutor", OracleDbType.Varchar2).Value = detalleTutor.Tutor;

                    

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {

                        return Ok(new
                        {
                            add = true
                        }) ;


                    }
                    else
                    {
                        return Ok(new { add = false });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }


        }

        [HttpGet("{id}")]
        public IActionResult ObtenerDetalleTutor(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from detalle_tutor where estudiante_id = :id";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":id", OracleDbType.Varchar2).Value = id;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 0)
                    {
                        string query = "SELECT * FROM detalle_tutor WHERE estudiante_id = :id";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                        OracleDataReader reader = command.ExecuteReader();

                        List<DetalleTutor> detalleTutores = new List<DetalleTutor>();

                        while (reader.Read())
                        {
                            DetalleTutor detalleTutor = new DetalleTutor()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                EstudianteId = Convert.ToInt32(reader["estudiante_id"]),
                                TutorId = Convert.ToInt32(reader["tutor_id"]),
                                Tutor = reader.GetString(reader.GetOrdinal("tutor"))
                            };

                            detalleTutores.Add(detalleTutor);
                        }

                        return Ok(new
                        {
                            get = true,
                            detalleTutor = detalleTutores
                        }); 

                    }else
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


        [HttpDelete("{id}")]
        public IActionResult EliminarDetalleTutor(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM detalle_tutor WHERE estudiante_id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { delete = true });
                    }
                    else
                    {
                        return Ok(new { delete = false });
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
