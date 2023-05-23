using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleApoderadoController : Controller
    {
       
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult AgregarDetalleApoderado(DetalleApoderado.DetalleApoderadoGet detalleApoderado)
        {
            try
            {


                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();


                    string query = "INSERT INTO detalle_apoderado (id, estudiante_id, apoderado_id, tutor) " +
                                   "VALUES (est_seq.NEXTVAL, :estudiante_id, :apoderado_id, :tutor) ";
                                  

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = detalleApoderado.EstudianteId;
                    command.Parameters.Add(":apoderado_id", OracleDbType.Int32).Value = detalleApoderado.ApoderadoId;
                    command.Parameters.Add(":tutor", OracleDbType.Varchar2).Value = detalleApoderado.Tutor;

                    

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
        public IActionResult ObtenerDetalleApoderado(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from detalle_apoderado  where estudiante_id = :id";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":id", OracleDbType.Varchar2).Value = id;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 0)
                    {

                        string query = "SELECT * FROM detalle_apoderado WHERE estudiante_id = :id";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                        OracleDataReader reader = command.ExecuteReader();

                        List<DetalleApoderado> detalleApoderados = new List<DetalleApoderado>();

                        while (reader.Read())
                        {
                            DetalleApoderado detalleApoderado = new DetalleApoderado()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                EstudianteId = Convert.ToInt32(reader["estudiante_id"]),
                                ApoderadoId = Convert.ToInt32(reader["apoderado_id"]),
                                Tutor = reader.GetString(reader.GetOrdinal("tutor"))
                            };

                            detalleApoderados.Add(detalleApoderado);
                        }

                        return Ok(new {
                            get = true,
                            detalleApoderado = detalleApoderados
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

        [HttpDelete("{id}")]
        public IActionResult EliminarDetalleApoderado(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM detalle_apoderado WHERE estudiante_id = :id";
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
