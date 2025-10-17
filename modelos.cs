using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    public class modelos
    {

    }
    public class nombre
    {
        //no hagan mucho caso de esta linea,
        //solamente es una solucion que consegui para que aparezca el nombre cuando haces login
        public string UserName { get; set; }
    }
    //esto si es importante, es para que no nos tiren la tabla
    public class Usuario
    {
     public int Id { get; set; }
     public string Name { get; set; }
     public string contra { get; set; }
     public string correo { get; set; }

    }
}
