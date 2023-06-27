using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
using System.Text;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;



namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatriculaController : Controller
    {
        private readonly string connectionString = "WALLET_LOCATION=C:\\Users\\hp\\Oracle\\network\\admin;DATA SOURCE=pnhdata_high;TNS_ADMIN=C:\\Users\\hp\\Oracle\\network\\admin;PERSIST SECURITY INFO=True;USER ID=SISTEMA;PASSWORD=f7svQgqYvczj;";

        [HttpGet("{estudiante_id}")]
        public IActionResult ObtenerMatriculaPorId(int estudiante_id)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT m.id AS matricula_id,TO_CHAR(m.fecha,'DD/MM/YYYY') AS fecha,m.valor AS valor,m.confirmada AS confirmada,m.pagada AS pagada,e.id AS estudiante_id,e.rut AS rut,e.p_nombre AS p_nombre,e.s_nombre AS s_nombre,e.ap_paterno AS ap_paterno,e.ap_materno AS ap_materno,e.correo,e.edad,e.curso_ingreso,e.genero,c.id AS curso_id , c.nombre AS curso_nombre FROM matricula m INNER JOIN estudiante e ON m.estudiante_id=e.id INNER JOIN curso c ON e.curso_id=c.id WHERE estudiante_id = :estudiante_id ";
                    OracleCommand command = new OracleCommand(query, connection);
                    command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = estudiante_id;

                    OracleDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {


                        Matricula matricula = new Matricula()
                        {
                            Id = Convert.ToInt32(reader["matricula_id"]),
                            Fecha = reader["fecha"].ToString(),
                            Valor = Convert.ToInt32(reader["valor"]),
                            Estudiante = new Estudiante.EstudianteResponse()
                            {
                                Id = Convert.ToInt32(reader["estudiante_id"]),
                                Rut = reader["rut"].ToString(),
                                P_nombre = reader["p_nombre"].ToString(),
                                S_nombre = reader["s_nombre"].ToString(),
                                Ap_paterno = reader["ap_paterno"].ToString(),
                                Ap_materno = reader["ap_materno"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Edad = Convert.ToInt32(reader["edad"]),
                                Curso_ingreso = reader["curso_ingreso"].ToString(),
                                Genero = reader["genero"].ToString(),
                                
                                Curso = new Curso()
                                {
                                    Id = Convert.ToInt32(reader["curso_id"]),
                                    Nombre = reader["curso_nombre"].ToString(),
                                },
                                
                            },
                            Confirmada = reader["confirmada"].ToString(),
                            Pagada = reader["pagada"].ToString(),



                        };

                        return Ok(new
                        {
                            get = true,
                            matricula = matricula
                        });
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

        [HttpPost("Pago")]
        public IActionResult pagarMatricula(Matricula.Pago pago_matricula)
        {


            string caracteresPermitidos = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ|_=&%.,~:/?[+!@()>-";
            int longitudMaxima = 26;

            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            while (sb.Length < longitudMaxima)
            {
                int index = random.Next(caracteresPermitidos.Length);
                sb.Append(caracteresPermitidos[index]);
            }

            string numeroOrdenCompra = sb.ToString();
           

            // Versión 4.x del SDK
            var tx = new Transaction(new Transbank.Common.Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var response = tx.Create(numeroOrdenCompra, "NuevosHorizontes", pago_matricula.Valor, "http://localhost:8100/matricula/" + pago_matricula.Estudiante_id);

           

            return Ok(new
            {
                Url = response.Url,
                Token = response.Token

            });




        }

       

        [HttpPost("Pago/Respuesta")]
        public IActionResult pagarMatriculaResultado(Matricula.Respuesta respuesta_pago)
        {

            


            var tx = new Transaction(new Transbank.Common.Options(IntegrationCommerceCodes.WEBPAY_PLUS, IntegrationApiKeys.WEBPAY, WebpayIntegrationType.Test));
            var response = tx.Commit(respuesta_pago.Token_ws);

            DateTime fechaTransaccion = (DateTime)response.TransactionDate;
            string fechaISO8601 = fechaTransaccion.ToString("o");
            DateTime fechaUTC = DateTime.Parse(fechaISO8601, null, System.Globalization.DateTimeStyles.RoundtripKind);

            TimeZoneInfo zonaHorariaChile = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time"); // Huso horario de Chile

            DateTime fechaChile = TimeZoneInfo.ConvertTimeFromUtc(fechaUTC, zonaHorariaChile);

            string fechaChileFormateada = fechaChile.ToString("yyyy-MM-dd HH:mm:ss");


            if (response.Status == "AUTHORIZED" && response.ResponseCode == 0)
            {
                try
                {
                    using (OracleConnection connection = new OracleConnection(connectionString))
                    {
                        connection.Open();

                        string query = "UPDATE  matricula SET pagada = 't' WHERE estudiante_id = :estudiante_id ";
                        OracleCommand command = new OracleCommand(query, connection);
                        command.Parameters.Add(":estudiante_id", OracleDbType.Int32).Value = respuesta_pago.Estudiante_id;

                        OracleDataReader reader = command.ExecuteReader();

                        reader.Read();



                        return Ok(new
                        {
                            pago = true,
                            datos = new
                            {
                                valor = response.Amount,
                                fecha = fechaChileFormateada,
                                numero_orden = response.BuyOrder
                            }



                        }) ;



                    }
                }
                catch (OracleException ex)
                {
                    return BadRequest(new { error = "Error al conectar a la base de datos de Oracle: " + ex.Message });
                }

            }
            else
            {
                return Ok(new
                {
                    pago = false
                });
            }
            

            


        }
    }
}
