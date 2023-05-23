using backend.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : Controller
    {
        

        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult AgregarEstudiante(Estudiante.EstudianteGet estudiante)
        {
            try
            {


                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    
                    string queryExists = "SELECT COUNT(*) FROM Estudiante WHERE rut = :rut";
                    OracleCommand commandExists = new OracleCommand(queryExists, connection);
                    commandExists.Parameters.Add(":rut", OracleDbType.Varchar2).Value = estudiante.Rut;
                    int existingCount = Convert.ToInt32(commandExists.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        return BadRequest(new { add = false });
                    }

                    string password = estudiante.P_nombre.Substring(0, 4) + "." + estudiante.Rut.Substring(estudiante.Rut.LastIndexOf('-') - 4, 4);

                    string correo = estudiante.P_nombre.Substring(0, 4).ToLower() + "." + estudiante.Ap_paterno.ToLower() + "@estudiante.nsh.cl";

                    string query = "INSERT INTO Estudiante (id,rut, p_nombre, s_nombre, ap_paterno, ap_materno, correo, password, edad, curso_ingreso, pendiente, genero) " +
                                   "VALUES (est_seq.NEXTVAL, :rut, :p_nombre, :s_nombre, :ap_paterno, :ap_materno, :correo, :password, :edad, :curso_ingreso, :pendiente, :genero) " +
                                   "RETURNING id INTO :newId";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = estudiante.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = estudiante.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = estudiante.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = estudiante.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = estudiante.Ap_materno;
                    command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = correo;
                    command.Parameters.Add(":password", OracleDbType.Varchar2).Value = password;
                    command.Parameters.Add(":edad", OracleDbType.Int32).Value = estudiante.Edad;
                    command.Parameters.Add(":curso_ingreso", OracleDbType.Varchar2).Value = estudiante.Curso_ingreso;
                    command.Parameters.Add(":pendiente", OracleDbType.Varchar2).Value = 't';
                    command.Parameters.Add(":genero", OracleDbType.Varchar2).Value = estudiante.Genero;

                    OracleParameter newIdParameter = new OracleParameter(":newId", OracleDbType.Int32);
                    newIdParameter.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(newIdParameter);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        OracleDecimal oracleDecimal = (OracleDecimal)newIdParameter.Value;
                        int newId = oracleDecimal.ToInt32();
                        return Ok(new {
                            add = true,
                            id = newId
                        });


                    }
                    else
                    {
                        return BadRequest(new { error = "No se pudo agregar el estudiante" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }

        
        }


        [HttpGet("{id}")]
        public IActionResult ObtenerEstudiantePorId(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Estudiante WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Estudiante.EstudianteResponse estudiante = new Estudiante.EstudianteResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Rut = reader["rut"].ToString(),
                            P_nombre = reader["p_nombre"].ToString(),
                            S_nombre = reader["s_nombre"].ToString(),
                            Ap_paterno = reader["ap_paterno"].ToString(),
                            Ap_materno = reader["ap_materno"].ToString(),
                            Correo = reader["correo"].ToString(),
                            Edad = Convert.ToInt32(reader["edad"]),
                            Curso_ingreso = reader["curso_ingreso"].ToString(),
                            Genero = reader["genero"].ToString()
                        };

                        return Ok(estudiante);
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró el estudiante con el id proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }

        }


        [HttpGet]
        public IActionResult ObtenerTodosLosEstudiantes()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Estudiante";
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Estudiante.EstudianteResponse> estudiantes = new List<Estudiante.EstudianteResponse>();

                    while (reader.Read())
                    {
                        Estudiante.EstudianteResponse estudiante = new Estudiante.EstudianteResponse()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Rut = reader["rut"].ToString(),
                            P_nombre = reader["p_nombre"].ToString(),
                            S_nombre = reader["s_nombre"].ToString(),
                            Ap_paterno = reader["ap_paterno"].ToString(),
                            Ap_materno = reader["ap_materno"].ToString(),
                            Correo = reader["correo"].ToString(),
                            Edad = Convert.ToInt32(reader["edad"]),
                            Curso_ingreso = reader["curso_ingreso"].ToString(),
                            Genero = reader["genero"].ToString()
                        };

                        estudiantes.Add(estudiante);
                    }

                    return Ok(estudiantes);
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarEstudiante(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Estudiante WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { delete = true});
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró ningún estudiante con el ID proporcionado" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult ModificarEstudiante(int id, [FromBody] Estudiante.EstudianteGet estudiante)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string queryExists = "SELECT COUNT(*) FROM Estudiante WHERE id = :id";
                    OracleCommand commandExists = new OracleCommand(queryExists, connection);
                    commandExists.Parameters.Add(":id", OracleDbType.Int32).Value = id;
                    int existingCount = Convert.ToInt32(commandExists.ExecuteScalar());

                    if (existingCount == 0)
                    {
                        return NotFound(new { error = "No se encontró ningún estudiante con el ID proporcionado" });
                    }

                    string query = "UPDATE Estudiante SET rut = :rut, p_nombre = :p_nombre, s_nombre = :s_nombre, " +
                                   "ap_paterno = :ap_paterno, ap_materno = :ap_materno, " +
                                   "edad = :edad, curso_ingreso = :curso_ingreso, genero = :genero " +
                                   "WHERE id = :id";

                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":rut", OracleDbType.Varchar2).Value = estudiante.Rut;
                    command.Parameters.Add(":p_nombre", OracleDbType.Varchar2).Value = estudiante.P_nombre;
                    command.Parameters.Add(":s_nombre", OracleDbType.Varchar2).Value = estudiante.S_nombre;
                    command.Parameters.Add(":ap_paterno", OracleDbType.Varchar2).Value = estudiante.Ap_paterno;
                    command.Parameters.Add(":ap_materno", OracleDbType.Varchar2).Value = estudiante.Ap_materno;
                    command.Parameters.Add(":edad", OracleDbType.Int32).Value = estudiante.Edad;
                    command.Parameters.Add(":curso_ingreso", OracleDbType.Varchar2).Value = estudiante.Curso_ingreso;
                    command.Parameters.Add(":genero", OracleDbType.Varchar2).Value = estudiante.Genero;
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { message = "Estudiante modificado exitosamente" });
                    }
                    else
                    {
                        return BadRequest(new { error = "No se pudo modificar el estudiante" });
                    }
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }

        [HttpGet("Pendientes")]
        public IActionResult ObtenerEstudiantesPendientes()
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Estudiante WHERE pendiente = 't'";
                    OracleCommand command = new OracleCommand(query, connection);

                    OracleDataReader reader = command.ExecuteReader();

                    List<Estudiante.EstudianteResponse> estudiantes = new List<Estudiante.EstudianteResponse>();

                    while (reader.Read())
                    {
                        Estudiante.EstudianteResponse estudiante = new Estudiante.EstudianteResponse
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Rut = reader["rut"].ToString(),
                            P_nombre = reader["p_nombre"].ToString(),
                            S_nombre = reader["s_nombre"].ToString(),
                            Ap_paterno = reader["ap_paterno"].ToString(),
                            Ap_materno = reader["ap_materno"].ToString(),
                            Correo = reader["correo"].ToString(),
                            Edad = Convert.ToInt32(reader["edad"]),
                            Curso_ingreso = reader["curso_ingreso"].ToString(),
                            Genero = reader["genero"].ToString()
                        };

                        estudiantes.Add(estudiante);
                    }

                    return Ok(estudiantes);
                }
            }
            catch (OracleException ex)
            {
                return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
            }
        }


        [HttpPut("cambiar-pendiente/{id}")]
        public IActionResult CambiarPendiente(int id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Estudiante SET pendiente = 'f' WHERE id = :id";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new { success = true });
                    }
                    else
                    {
                        return NotFound(new { error = "No se encontró el estudiante con el ID proporcionado" });
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


