
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApoderadoController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult agregarApoderado(Apoderado.ApoderadoGet apoderado)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from apoderado where rut = :rut";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":rut", OracleDbType.Varchar2).Value = apoderado.Rut;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 0)
                    {
                        string querygetid = "select id from apoderado where rut = :rut";
                        OracleCommand commandgetid = new OracleCommand(querygetid, connection);
                        commandgetid.Parameters.Add(":rut", OracleDbType.Varchar2).Value = apoderado.Rut;
                        int existingid = Convert.ToInt32(commandgetid.ExecuteScalar());

                        return Ok(new
                        {
                            exists = true,
                            id = existingid
                        });
                    }


                    string password = apoderado.P_nombre.Substring(0, 4) + "." + apoderado.Rut.Substring(apoderado.Rut.LastIndexOf('-') - 4, 4);
                    string correo = apoderado.P_nombre.Substring(0, 4).ToLower() + "." + apoderado.Ap_paterno.ToLower() + "@apoderado.nsh.cl";

                    string query = "insert into apoderado (id, rut, p_nombre, s_nombre, ap_paterno, ap_materno, correo, password, celular) " +
                                   "values (apd_seq.nextval, :rut, :p_nombre, :s_nombre, :ap_paterno, :ap_materno, :correo, :password,:celular) " +
                                   "returning id into :newid";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = apoderado.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = apoderado.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = apoderado.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = apoderado.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = apoderado.Ap_materno;
                    command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = correo;
                    command.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;
                    command.Parameters.Add(":celular", OracleDbType.Int32).Value = apoderado.Celular;

                    OracleParameter newidparameter = new OracleParameter(":newid", OracleDbType.Int32);
                    newidparameter.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(newidparameter);

                    int rowsaffected = command.ExecuteNonQuery();

                    if (rowsaffected > 0)
                    {
                        OracleDecimal oracledecimal = (OracleDecimal)newidparameter.Value;
                        int newid = oracledecimal.ToInt32();
                        return Ok(new
                        {
                            add = true,
                            id = newid
                        });
                    }
                    else
                    {
                        return BadRequest(new { error = "no se pudo agregar el estudiante" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "error al conectar a la base de datos de oracle: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarApoderado(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from detalle_apoderado  where apoderado_id = :id";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":id", OracleDbType.Varchar2).Value = id;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 0)
                    {
                        
                        return Ok(new
                        {
                            delete = false

                        });
                    }

                    string query = "DELETE FROM Apoderado WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new
                        {
                            delete = true
                        });
                    }
                    else
                    {
                        return BadRequest(new { error = "No se encontró un apoderado con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerApoderado(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Apoderado WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Apoderado.ApoderadoResponse apoderado = new Apoderado.ApoderadoResponse
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Rut = reader.GetString(reader.GetOrdinal("rut")),
                            P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                            S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                            Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                            Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                            Correo = reader.GetString(reader.GetOrdinal("correo")),
                            Celular = reader.GetInt32(reader.GetOrdinal("celular"))
                        };

                        return Ok(apoderado);
                    }
                    else
                    {
                        return BadRequest(new { error = "No se encontró un apoderado con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ActualizarApoderado(int id, Apoderado.ApoderadoGet apoderado)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Apoderado SET rut = :rut, p_nombre = :p_nombre, s_nombre = :s_nombre, " +
                                   "ap_paterno = :ap_paterno, ap_materno = :ap_materno, celular = :celular " +
                                   "WHERE id = :id";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = apoderado.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = apoderado.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = apoderado.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = apoderado.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = apoderado.Ap_materno;
                    command.Parameters.Add(":celular", OracleDbType.Int32).Value = apoderado.Celular;
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new
                        {
                            update = true
                        });
                    }
                    else
                    {
                        return BadRequest(new { error = "No se encontró un apoderado con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ObtenerTodosLosApoderados()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Apoderado";
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Apoderado.ApoderadoResponse> apoderados = new List<Apoderado.ApoderadoResponse>();

                    while (reader.Read())
                    {
                        Apoderado.ApoderadoResponse apoderado = new Apoderado.ApoderadoResponse
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Rut = reader.GetString(reader.GetOrdinal("rut")),
                            P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                            S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                            Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                            Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                            Correo = reader.GetString(reader.GetOrdinal("correo")),
                            Celular = reader.GetInt32(reader.GetOrdinal("celular"))
                        };

                        apoderados.Add(apoderado);
                    }

                    return Ok(apoderados);
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }


    }
}
