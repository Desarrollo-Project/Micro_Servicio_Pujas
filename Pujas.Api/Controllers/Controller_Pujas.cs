using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pujas.Aplicacion.Commands;
using Pujas.Aplicacion.DTO;
using Pujas.Aplicacion.Validaciones;
using Pujas.Dominio.Entidades;
using Pujas.Dominio.Objetos_De_Valor;
using Pujas.Dominio.Repositorios;


namespace Pujas.Api.Controllers
{
    public class Controller_Pujas : ControllerBase
    {
        private readonly IMediator _mediador;
        private readonly IHubContext<Pujas_Hub> _hub_Context;
        private readonly IRepositorio_Pujas_Lectura _pujas_lectura;

        public Controller_Pujas(IMediator m, IHubContext<Pujas_Hub> hub, IRepositorio_Pujas_Lectura pjl)
        {
            _mediador = m;
            _hub_Context = hub;
            _pujas_lectura = pjl;
        }


    /// <summary>
    /// Obtiene el historial completo de pujas para una subasta específica, ordenado por fecha.
    /// </summary>
    /// <param name="idSubasta">El identificador único de la subasta.</param>
    /// <returns>
    /// Una lista de objetos que representan el historial de pujas si la operación es exitosa (200 OK).
    /// Devuelve un error 500 si ocurre un problema en el servidor.
    /// </returns>
    /// 
    [HttpGet("Historial/{idSubasta}")]

        [HttpGet("Historial/{idSubasta}")]
        public async Task<IActionResult> ObtenerHistorialPujas(string idSubasta)
        {
            try
            {
                var historialPujas = await _pujas_lectura.Obtener_Pujas_Subasta_Por_Orden(idSubasta);
                var pujasFrontend = historialPujas.Select(p => new
                {
                    id = p.Id.ToString(),
                    auctionId = p.Id_Subasta.id_Subasta,
                    amount = p.Monto,
                    bidder = p.Id_Postor,
                    timestamp = p.Fecha_Puja,
                }).ToList();

                return Ok(pujasFrontend);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor al obtener el historial de pujas.");
            }
        }


         /// <summary>
    /// Registra una nueva puja manual para una subasta. Valida que la subasta esté activa y que el monto sea superior a la última puja.
    /// Después de crear la puja, notifica a los clientes conectados a través de SignalR y procesa las pujas automáticas correspondientes.
    /// </summary>
    /// <param name="dto">Objeto DTO (Data Transfer Object) con los datos para crear la nueva puja, como el ID de la subasta, el ID del postor y el monto.</param>
    /// <returns>
    /// Devuelve 200 OK si la puja se crea correctamente.
    /// Devuelve 400 Bad Request si la subasta no está activa, si el monto no es válido o si hay un error en la creación.
    /// </returns>
    /// 
        [HttpPost("Crear_Puja")]
        public async Task<IActionResult> Crear_Puja([FromBody] Crear_Puja_DTO dto)
        {
            try
            {
                var command = new Crear_Puja_Command(dto);
                var monto_Ultima_Puja = await _pujas_lectura.Obtener_Ultimo_Monto_Puja(dto.Id_Subasta.ToString());
                Console.WriteLine(monto_Ultima_Puja);

                var validaciones = new Crear_Puja_Validaciones(new HttpClient());
                var Estado_subasta = await validaciones.Subasta_Esta_Activa_Async(dto.Id_Subasta.ToString());


               // Validaciones antes de hacer la Puja 
                if (!Estado_subasta) return BadRequest("La subasta ya no se encuentra activa");
                if (dto.Monto <= monto_Ultima_Puja) {return BadRequest("El monto de la puja debe ser mayor al monto de la ultima puja"); }

                var resultado = await _mediador.Send(command);
                if (resultado == null) return BadRequest("No se pudo crear la puja");
                await _hub_Context.Clients.Group(dto.Id_Subasta)
                    .SendAsync(
                        "PrecioActualizado",
                        dto.Monto,
                        dto.Id_Postor,
                        false
                    );


                var lista_Pujas = await _pujas_lectura.Obtener_Pujas_Automaticas_Subasta_Por_Orden(dto.Id_Subasta);
                var monto = await _pujas_lectura.Obtener_Ultimo_Monto_Puja(dto.Id_Subasta);

                if (lista_Pujas.Count > 0)
                {
                    foreach (var puja in lista_Pujas)
                    {
                        if (puja.Monto_Maximo_valido.HasValue && puja.Monto_Maximo_valido < monto + puja.Incremento.Incremento) continue;

                        monto = monto + puja.Incremento.Incremento;
                        var nueva_puja_automatica_dto = new Crear_Puja_DTO(
                            id: Guid.NewGuid(),
                            id_Subasta: puja.Id_Subasta.id_Subasta,
                            id_Postor: puja.Id_Postor.id_postor,
                            fecha_Puja: DateTime.UtcNow,
                            monto: monto,
                            incremento: puja.Incremento.Incremento
                        );

                        var nueva_puja_command = new Crear_Puja_Command(nueva_puja_automatica_dto);
                        var resultado_puja_automatica = await _mediador.Send(nueva_puja_command);

                        if (resultado_puja_automatica != null)
                        {
                            await _hub_Context.Clients.Group(puja.Id_Subasta.id_Subasta)
                                .SendAsync(
                                    "PrecioActualizado", monto,
                                    puja.Id_Postor.id_postor, true);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

            /// <summary>
            /// Configura y guarda una puja automática para un usuario en una subasta específica.
            /// </summary>
            /// <param name="dto">Objeto DTO con los datos para la puja automática, incluyendo el postor, la subasta, el monto máximo y el incremento.</param>
            /// <returns>
            /// Devuelve 200 OK con el objeto de la puja automática creada si la operación es exitosa.
            /// Devuelve 400 Bad Request si ocurre una excepción durante el proceso.
            /// </returns>
                // Crear Puja automatica 
        [HttpPost("Crear_Puja_Automatica")]
        public async Task<IActionResult> Crear_Puja_Automatica([FromBody] Crear_Puja_Automatica_DTO dto)
        {
            try
            {
                // Faltan validaciones puja automatica 

                var puja = new Puja_Mongo(
                    Guid.NewGuid().ToString(),
                    new Id_Postor_VO(dto.Id_Postor),
                    new Id_Subasta_VO(dto.Id_Subasta),
                    new Fecha_Puja_VO(dto.Fecha_Creacion_Puja_Automatica),
                    new Incremento_VO(dto.Incrementos),
                    dto.Maximo_Monto
                );
                await _pujas_lectura.Crear_Puja_Automatizada(puja);
                return Ok(puja);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

         /// <summary>
    /// Ejecuta el proceso de realizar pujas automáticas para una subasta, generalmente invocado por un temporizador o un evento del sistema.
    /// </summary>
    /// <param name="id_sub">El identificador único de la subasta para la cual se procesarán las pujas automáticas.</param>
    /// <returns>
    /// Devuelve 200 OK si el proceso se completa.
    /// Devuelve 400 Bad Request si ocurre una excepción.
    /// </returns>

        [HttpPost("Hacer_Pujas_Automaticas")]
        public async Task<IActionResult> Hacer_Pujas_Automaticas_Por_Temporizador([FromBody]  string id_sub)
        {
            try
            {
                var lista_Pujas = await _pujas_lectura.Obtener_Pujas_Automaticas_Subasta_Por_Orden(id_sub);
                var monto = await _pujas_lectura.Obtener_Ultimo_Monto_Puja(id_sub);
                var id_Postor_Ultima_Puja = await _pujas_lectura.Obtener_Id_Postor_Ultima_Puja(id_sub);
                

                if (lista_Pujas.Count > 0)
                {
                    foreach (var puja in lista_Pujas)
                    {
                        // Evitar que se haga una puja automatica si el monto maximo es menor al monto actual + incremento
                        if (puja.Monto_Maximo_valido < monto + puja.Incremento.Incremento) continue;
                       
                        // Evitar que el postor haga una puja automatica si ya hizo la ultima puja
                        if ( puja.Id_Postor.id_postor == id_Postor_Ultima_Puja) continue;
                        Console.WriteLine(puja);

                        monto = monto + puja.Incremento.Incremento;

                        var nueva_puja_automatica_dto = new Crear_Puja_DTO(
                            id: Guid.NewGuid(),
                            id_Subasta: puja.Id_Subasta.id_Subasta,
                            id_Postor: puja.Id_Postor.id_postor,
                            fecha_Puja: DateTime.UtcNow,
                            monto: monto,
                            incremento: puja.Incremento.Incremento
                        );

                        var nueva_puja_command = new Crear_Puja_Command(nueva_puja_automatica_dto);
                        var resultado_puja_automatica = await _mediador.Send(nueva_puja_command);

                        if (resultado_puja_automatica != null)
                        {
                            await _hub_Context.Clients.Group(puja.Id_Subasta.id_Subasta)
                                .SendAsync(
                                    "PrecioActualizado", monto,
                                    puja.Id_Postor.id_postor, true);
                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
            /// Comprueba si un usuario específico ya tiene una configuración de puja automática activa en alguna subasta.
            /// </summary>
            /// <param name="id_postor">El identificador único del postor (usuario).</param>
            /// <returns>
            /// Devuelve 200 OK con un valor booleano: 'true' si el usuario tiene una puja automática, 'false' en caso contrario.
            /// Devuelve 400 Bad Request si ocurre una excepción.
            /// </returns>
        [HttpGet("Verificar_Si_Usuario_tiene_Puja_Automatica/{id_postor}")]
        public async Task<IActionResult> Verificar_Si_Usuario_tiene_Puja_Automatica(string id_postor)
        {
            try
            {
                var tienePujaAutomatica = await _pujas_lectura.Comprobar_Si_Usuario_Tiene_Puja_Automatica(id_postor);
                return Ok(tienePujaAutomatica);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



     /// <summary>
    /// Obtiene los detalles de la puja más reciente (la más alta) para una subasta específica.
    /// </summary>
    /// <param name="id_subasta">El identificador único de la subasta.</param>
    /// <returns>
    /// Devuelve 200 OK con un objeto que contiene el nombre del ofertante y el monto de la última puja.
    /// Devuelve 400 Bad Request si ocurre una excepción.
    /// </returns>
    /// 
        [HttpGet("Obtener_Ultima_Puja_Subasta/{id_subasta}")]
        public async Task<IActionResult> Obtener_Ultima_Puja_Subasta(string id_subasta)
        {
            try
            {
                var ultimaPuja = await _pujas_lectura.Obtener_Datos_Ultima_Puja(id_subasta);
                return Ok(new
                {
                    NombreOfertante = ultimaPuja.nombreOfertante,
                    UltimaPuja = ultimaPuja.ultimaPuja
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
