using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaController : Controller
    {

        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult agregarSala(Sala.SalaGet sala)
        {
            try
            {


                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();


                    string queryExists = "SELECT COUNT(*) FROM sala WHERE numero = :numero";
                    OracleCommand commandExists = new OracleCommand(queryExists, connection);
                    commandExists.Parameters.Add(":numero", OracleDbType.Int32).Value = sala.Numero;
                    int existingCount = Convert.ToInt32(commandExists.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        return Ok(new { add = false });
                    }


                    string query = "INSERT INTO sala (id,numero) " +
                                   "VALUES (sla_seq.NEXTVAL, :numero) " +
                                   "RETURNING id INTO :newId";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":numero", OracleDbType.Varchar2).Value = sala.Numero;



                    OracleParameter newIdParameter = new OracleParameter(":newId", OracleDbType.Int32);
                    newIdParameter.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(newIdParameter);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        OracleDecimal oracleDecimal = (OracleDecimal)newIdParameter.Value;
                        int newId = oracleDecimal.ToInt32();
                        return Ok(new
                        {
                            add = true,
                            id = newId
                        });


                    }
                    else
                    {
                        return BadRequest(new { error = "No se pudo agregar la sala" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }


        }

        [HttpGet("{id}")]
        public IActionResult obtenerSala(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM sala WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                       
                        Sala.SalaResponse sala = new Sala.SalaResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Numero = Convert.ToInt32(reader["numero"]),

                        };

                        return Ok(new{
                            Get = true,
                            Sala = sala
                        });
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró la sala con el id proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }

        }

        [HttpGet]
        public IActionResult obtenerTodosLasSalas()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM sala";
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Sala.SalaResponse> salas = new List<Sala.SalaResponse>();

                    while (reader.Read())
                    {
                        
                        Sala.SalaResponse sala = new Sala.SalaResponse()
                        {


                            Id = Convert.ToInt32(reader["id"]),
                            Numero = Convert.ToInt32(reader["numero"]),
                            



                        };

                        salas.Add(sala);
                    }

                    return Ok(new{ 
                        Get = true,
                        Salas = salas});
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }
    }
}
