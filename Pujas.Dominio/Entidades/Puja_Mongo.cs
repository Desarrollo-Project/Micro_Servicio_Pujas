using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pujas.Dominio.Objetos_De_Valor;

namespace Pujas.Dominio.Entidades
{

    public class Puja_Mongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public Id_Postor_VO Id_Postor { get; set; }
        public Id_Subasta_VO Id_Subasta { get; set; }
        public Fecha_Puja_VO Fecha_Puja { get; set; }

        public Monto_Total_VO Monto { get; set; }
        public Incremento_VO Incremento { get; set; }
        public decimal? Monto_Maximo_valido { get; set; }


        // Constructor para Pujas Manuales
        public Puja_Mongo(string id, Id_Postor_VO id_postor, Id_Subasta_VO id_subasta,
            Fecha_Puja_VO fecha_puja, Monto_Total_VO monto, Incremento_VO incremento)
        {
            Id = id;
            Id_Postor = id_postor;
            Id_Subasta = id_subasta;
            Fecha_Puja = fecha_puja;
            Monto = monto;
            Monto_Maximo_valido = null; 
        }

        // Constructor para Pujas Automáticas
        public Puja_Mongo(string id, Id_Postor_VO id_postor, Id_Subasta_VO id_subasta,
            Fecha_Puja_VO fecha_puja, Incremento_VO incremento, decimal maximoMonto) 
        {
            Id = id;
            Id_Postor = id_postor;
            Id_Subasta = id_subasta;
            Fecha_Puja = fecha_puja;
            Incremento = incremento;
            Monto_Maximo_valido = maximoMonto; 
            Monto = null; 
        }
    }




}
