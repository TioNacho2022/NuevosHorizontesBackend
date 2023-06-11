using Microsoft.AspNetCore.Mvc;
using backend.Models;
using Oracle.ManagedDataAccess.Client;

namespace backend.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpPost]
        public IActionResult Login(Credencial credencial)
        {
            Console.Write(credencial.Correo);

            string nombreUsuario = ObtenerNombreUsuario(credencial.Correo);

            static string ObtenerNombreUsuario(string correoElectronico)
            {
                int indiceArroba = correoElectronico.IndexOf("@");
                int indicePunto = correoElectronico.IndexOf(".", indiceArroba);

                if (indiceArroba >= 0 && indicePunto >= 0)
                {
                    int longitud = indicePunto - (indiceArroba + 1);
                    string nombreUsuario = correoElectronico.Substring(indiceArroba + 1, longitud);
                    char primeraLetra = char.ToUpper(nombreUsuario[0]);
                    string restoTexto = nombreUsuario.Substring(1).ToLower();

                    return nombreUsuario = primeraLetra+restoTexto;
                }

                return null;

                

            }

            


            if (nombreUsuario == "Apoderado")
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM apoderado WHERE correo = :correo AND password= :password ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = credencial.Correo;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = credencial.Password;

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

                            return Ok(new { 
                                   auth = true,
                                   rol= nombreUsuario,
                                   usuario = apoderado
                                });
                        }
                        else
                        {
                            return Ok(new { auth = false });
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }


            }if(nombreUsuario == "Estudiante")
            {
                try
                {
                   

                   
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        Console.Write(nombreUsuario);

                        string query = "SELECT * FROM estudiante WHERE correo = :correo AND password= :password ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = credencial.Correo;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = credencial.Password;

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            Estudiante.EstudianteResponse estudiante = new Estudiante.EstudianteResponse
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Rut = reader.GetString(reader.GetOrdinal("rut")),
                                P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                                S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                                Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                                Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                                Correo = reader.GetString(reader.GetOrdinal("correo")),
                                Edad = reader.GetInt32(reader.GetOrdinal("edad")),
                                Curso_ingreso = reader.GetString(reader.GetOrdinal("curso_ingreso")),
                                Genero = reader.GetString(reader.GetOrdinal("genero")),

                            };

                            return Ok(new
                                {
                                auth = true,
                                rol = nombreUsuario,
                                usuario = estudiante
                                });
                        }
                        else
                        {
                            return Ok(new { auth = false });
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }
            }if(nombreUsuario == "Tutor")
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM tutor WHERE correo = :correo AND password= :password ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = credencial.Correo;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = credencial.Password;

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

                            return Ok(new
                            {
                                auth = true,
                                rol = nombreUsuario,
                                usuario = tutor
                            });
                        }
                        else
                        {
                            return Ok(new { auth = false });
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }
            }
            if (nombreUsuario == "Empleado")
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM empleado INNER JOIN rol USING(id) WHERE correo = :correo AND password= :password ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = credencial.Correo;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = credencial.Password;

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            Empleado.EmpleadoResponse empleado = new Empleado.EmpleadoResponse
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Rut = reader.GetString(reader.GetOrdinal("rut")),
                                P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                                S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                                Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                                Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                                Correo = reader.GetString(reader.GetOrdinal("correo")),
                                Rol = new Rol
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                }
                               
                            };

                            return Ok(new
                            {
                                auth = true,
                                rol = empleado.Rol.Nombre,
                                usuario = empleado
                            });
                        }
                        else
                        {
                            return Ok(new { auth = false });
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }
            }
            if (nombreUsuario == "Profesor")
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM profesor WHERE correo = :correo AND password= :password ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = credencial.Correo;
                        command.Parameters.Add(":password", OracleDbType.Varchar2).Value = credencial.Password;

                        OracleDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            Profesor.Response profesor = new Profesor.Response
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Rut = reader.GetString(reader.GetOrdinal("rut")),
                                P_nombre = reader.GetString(reader.GetOrdinal("p_nombre")),
                                S_nombre = reader.GetString(reader.GetOrdinal("s_nombre")),
                                Ap_paterno = reader.GetString(reader.GetOrdinal("ap_paterno")),
                                Ap_materno = reader.GetString(reader.GetOrdinal("ap_materno")),
                                Correo = reader.GetString(reader.GetOrdinal("correo")),
                               

                            };

                            return Ok(new
                            {
                                auth = true,
                                rol = nombreUsuario,
                                usuario = profesor
                            });
                        }
                        else
                        {
                            return Ok(new { auth = false });
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }
            }
            return null;
        }
    }
}
