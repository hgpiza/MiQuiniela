using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using MiQuiniela.Models;

namespace MiQuiniela.Controllers
{
    public class HomeController : Controller
    {
        QuinielaEntities db = new QuinielaEntities();

        public ActionResult Index()
        {
            var accessToken = Session["AccessToken"].ToString();
            var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me");
            string userName = me.username;
            string firstName = me.first_name;
            string id = me.id;
            string email = me.email;

            var queryUsuario = from u in db.Usuarios
                               where u.ID_Facebook == id
                               select u;
            
            Usuario usuario = new Usuario();

            if (queryUsuario.Count() == 0)
            {
                usuario = new Usuario();
                if (db.Usuarios.OrderByDescending(u => u.ID_Usuario).Count() > 0)
                    usuario.ID_Usuario = db.Usuarios.OrderByDescending(u => u.ID_Usuario).FirstOrDefault().ID_Usuario + 1;
                else
                    usuario.ID_Usuario = 1;
                usuario.ID_Facebook = id;
                usuario.FechaIns = DateTime.Now;
                usuario.Email = email;
                usuario.Nickname = userName;
                try
                {
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
                usuario = queryUsuario.FirstOrDefault();

            var queryGrupos = from g in db.UsuarioGrupoes
                              where g.ID_Usuario == usuario.ID_Usuario
                                    && g.Confirmado == true
                                    && g.ID_Grupo == 2
                              select g;
            //queryGrupos = queryGrupos.GroupBy(g => g.ID_Grupo).Select(gr => gr.FirstOrDefault());
            if (queryGrupos.Count() <= 0)
                db.UsuarioGrupoes.Add(new UsuarioGrupo
                {
                    ID_Usuario = usuario.ID_Usuario,
                    ID_Grupo = 2,
                    Usuario = usuario,
                    Confirmado = true,
                    TotalPuntos = 0
                });
            db.SaveChanges();
            TempData["ID_Usuario"] = usuario.ID_Usuario;
            ViewBag.Message = "Bienvenido(a) " + firstName + " a la Quiniela del Mundial.";
            //List<Grupos> Grupos = new List<Grupos>();
            //foreach (UsuarioGrupo ug in queryGrupos)
            //{
            //    Grupos.Add(new Grupos
            //                {
            //                    ID_Grupo = ug.ID_Grupo,
            //                    Nombre = ug.Grupos.Nombre,
            //                    ID_Usuario = ug.ID_Usuario,
            //                    FechaIns = ug.Grupos.FechaIns,
            //                    Usuario = ug.Usuario
            //                });
            //}
            //return View(Grupos);
            if (String.IsNullOrEmpty(usuario.Nickname))
            {
                return RedirectToAction("Nickname", "Quiniela", new { ID_Usuario = usuario.ID_Usuario });
            }
            return RedirectToAction("Rondas", "Quiniela", new { ID_Grupo = 2, ID_Usuario = usuario.ID_Usuario, token = accessToken });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
