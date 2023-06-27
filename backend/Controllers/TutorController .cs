
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult agregarTutor(Tutor.TutorGet tutor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from tutor where rut = :rut";
                    OracleCommand commandexists = new OracleCommand(queryexists, connection);
                    commandexists.Parameters.Add(":rut", OracleDbType.Varchar2).Value = tutor.Rut;
                    int existingcount = Convert.ToInt32(commandexists.ExecuteScalar());

                    if (existingcount > 0)
                    {
                        string querygetid = "select id from tutor where rut = :rut";
                        OracleCommand commandgetid = new OracleCommand(querygetid, connection);
                        commandgetid.Parameters.Add(":rut", OracleDbType.Varchar2).Value = tutor.Rut;
                        int existingid = Convert.ToInt32(commandgetid.ExecuteScalar());

                        return Ok(new
                        {
                            exists = true,
                            id = existingid
                        });
                    }


                    string password = tutor.P_nombre.Substring(0, 4) + "." + tutor.Rut.Substring(tutor.Rut.LastIndexOf('-') - 4, 4);
                    string correo = tutor.P_nombre.Substring(0, 4).ToLower() + "." + tutor.Ap_paterno.ToLower() + "@tutor.nsh.cl";

                    string query = "insert into tutor (id, rut, p_nombre, s_nombre, ap_paterno, ap_materno, correo, password, celular) " +
                                   "values (apd_seq.nextval, :rut, :p_nombre, :s_nombre, :ap_paterno, :ap_materno, :correo, :password,:celular) " +
                                   "returning id into :newid";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = tutor.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = tutor.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = tutor.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = tutor.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = tutor.Ap_materno;
                    command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = correo;
                    command.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;
                    command.Parameters.Add(":celular", OracleDbType.Int32).Value = tutor.Celular;

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
                        return BadRequest(new { error = "no se pudo agregar el tutor" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "error al conectar a la base de datos de oracle: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarTutor(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryexists = "select count(*) from detalle_tutor  where tutor_id = :id";
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

                    string query = "DELETE FROM tutor WHERE id = :id";
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
                        return BadRequest(new { error = "No se encontró un tutor con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerTutor(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM tutor WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Tutor.TutorResponse tutor = new Tutor.TutorResponse
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

                        return Ok(new{
                            get = true,
                            rol = "Tutor",
                            usuario = tutor});
                    }
                    else
                    {
                        return BadRequest(new { error = "No se encontró un tutor con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ActualizarApoderado(int id, Tutor.TutorGet tutor)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE tutor SET rut = :rut, p_nombre = :p_nombre, s_nombre = :s_nombre, " +
                                   "ap_paterno = :ap_paterno, ap_materno = :ap_materno, celular = :celular " +
                                   "WHERE id = :id";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = tutor.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = tutor.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = tutor.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = tutor.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = tutor.Ap_materno;
                    command.Parameters.Add(":celular", OracleDbType.Int32).Value = tutor.Celular;
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
                        return BadRequest(new { error = "No se encontró un tutor con el ID proporcionado" });
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

                    string query = "SELECT * FROM tutor";
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Tutor.TutorResponse> tutores = new List<Tutor.TutorResponse>();

                    while (reader.Read())
                    {
                        Tutor.TutorResponse tutor = new Tutor.TutorResponse
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

                        tutores.Add(tutor);
                    }

                    return Ok(tutores);
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet("{id}/Estudiantes")]
        public IActionResult ObtenerTutoresEstudiantes(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM detalle_tutor WHERE tutor_id= :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    List<DetalleTutor.DetalleTutorGet> detalles_tutor = new List<DetalleTutor.DetalleTutorGet>();

                    while (reader.Read())
                    {
                        DetalleTutor.DetalleTutorGet detalle_tutor = new DetalleTutor.DetalleTutorGet
                        {
                            EstudianteId = reader.GetInt32(reader.GetOrdinal("estudiante_id")),

                        };

                        detalles_tutor.Add(detalle_tutor);
                    }

                    List<Estudiante.EstudianteResponse> estudiantes = new List<Estudiante.EstudianteResponse>();

                    foreach (var estudiante in detalles_tutor)
                    {

                        string query1 = "SELECT * FROM estudiante WHERE id=:id";

                        OracleCommand command1 = new OracleCommand(query1, connection);
                        command1.Parameters.Add(":id", OracleDbType.Int32).Value = estudiante.EstudianteId;
                        OracleDataReader reader1 = command1.ExecuteReader();

                        if (reader1.Read())
                        {
                            Estudiante.EstudianteResponse estudiante1 = new Estudiante.EstudianteResponse
                            {
                                Id = reader1.GetInt32(reader1.GetOrdinal("id")),
                                Rut = reader1.GetString(reader1.GetOrdinal("rut")),
                                P_nombre = reader1.GetString(reader1.GetOrdinal("p_nombre")),
                                S_nombre = reader1.GetString(reader1.GetOrdinal("s_nombre")),
                                Ap_paterno = reader1.GetString(reader1.GetOrdinal("ap_paterno")),
                                Ap_materno = reader1.GetString(reader1.GetOrdinal("ap_materno"))


                            };

                            estudiantes.Add(estudiante1);
                        }






                    }
                    return Ok(new
                    {
                        add = true,
                        estudiantes = estudiantes
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
