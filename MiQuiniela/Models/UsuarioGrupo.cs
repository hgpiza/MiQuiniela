//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiQuiniela.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsuarioGrupo
    {
        public int ID_Usuario { get; set; }
        public int ID_Grupo { get; set; }
        public Nullable<int> TotalPuntos { get; set; }
        public Nullable<bool> Confirmado { get; set; }
    
        public virtual Grupos Grupos { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
