using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Pujas.Aplicacion.DTO;

namespace Pujas.Aplicacion.Commands
{
    public class Crear_Puja_Command: IRequest<Guid>
    {
        public Crear_Puja_DTO dto { get; set; }
        public Crear_Puja_Command(Crear_Puja_DTO dto) {this.dto = dto; }
    }
}
