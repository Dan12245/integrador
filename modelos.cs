using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace CRA
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
    //esta clase guarda el email para usarlo despues en el codigo y sacar el id del usuario
    public static class mail
    {
        public static string email { get; set; }
    }
    //esto si es importante, es para que no nos tiren la tabla
    //olvidenlo no es tan importante porque ya tenemos los insert con variables, los @
    public class Usuario
    {
     public int Id { get; set; }
     public string Name { get; set; }
     public string contra { get; set; }
     public string correo { get; set; }

    }
}
