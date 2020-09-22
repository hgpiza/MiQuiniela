using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiQuiniela.Models;

namespace MiQuiniela.Controllers
{
    public class QuinielaController : Controller
    {
        //
        // GET: /Quiniela/
        private QuinielaEntities db = new QuinielaEntities();

        public ActionResult Rondas(int ID_Grupo, int ID_Usuario, string token)
        {
            //int idUsuario = Int32.Parse(TempData["ID_Usuario"].ToString());
            Session["AccessToken"] = token;
            int idUsuario = ID_Usuario;
            foreach (Partido p in db.Partidos)
            {
                if (db.Quinielas.Where(q => q.ID_Usuario == idUsuario)
                    .Where(g => g.ID_Grupo == ID_Grupo)
                    .Where(pa => pa.ID_Partido == p.ID_Partido).Count() == 0)
                {
                    db.Quinielas.Add(new Quiniela
                    {
                        ID_Usuario = idUsuario,
                        FechaIns = DateTime.Now,
                        ID_Partido = p.ID_Partido,
                        ID_Grupo = ID_Grupo,
                        Partido = p
                    });
                }
            }
            db.SaveChanges();
            var quinielas = db.Quinielas.Where(q => q.ID_Usuario == idUsuario)
                                .Where(g=>g.ID_Grupo==ID_Grupo);
            ViewBag.Title = db.Grupos.Where(g => g.ID_Grupo == ID_Grupo).FirstOrDefault().Nombre;
            
            ViewBag.Message = "Bienvenido(a) a la Quiniela del Mundial.";
            var rondas = (from b in quinielas
                         group b by b.Partido.Ronda into g
                         select g.FirstOrDefault()).OrderBy(r=>r.ID_Partido);

            return View(rondas.ToList());
        }

        public ActionResult Index(int ID_Grupo, int ID_Usuario, string Ronda)
        {
            //int idUsuario = Int32.Parse(TempData["ID_Usuario"].ToString());
            int idUsuario = ID_Usuario;
            
            var quinielas = db.Quinielas.Where(q => q.ID_Usuario == idUsuario)
                            .Where(g => g.ID_Grupo == ID_Grupo)
                            .Where(r=>r.Partido.Ronda == Ronda);
            ViewBag.Title = "Mi Quiniela";
            ViewBag.Message = Ronda;

            int ID_Partido = quinielas.ToList()[0].ID_Partido;
            string ronda = (from p in db.Partidos
                            where p.ID_Partido == ID_Partido
                            select p).FirstOrDefault().Ronda;

            var fecha = (from f in db.Partidos
                         where f.Ronda == ronda
                         orderby f.Fecha ascending
                         select f).FirstOrDefault().Fecha;

            if (fecha.Value.AddHours(-4) > DateTime.Now)
                return View(quinielas.ToList());
            else
            {
                TempData["Quinielas"] = quinielas.ToList();
                return RedirectToAction("Detalles");
            }
        }

        [HttpPost]
        public ActionResult Index(List<Quiniela> quinielas)
        {
            if (ModelState.IsValid)
            {
                int ID_Partido = quinielas[0].ID_Partido; 
                string ronda = (from p in db.Partidos
                               where p.ID_Partido == ID_Partido
                               select p).FirstOrDefault().Ronda;
                    
                var fecha = (from f in db.Partidos
                             where f.Ronda == ronda
                            orderby f.Fecha ascending
                            select f).FirstOrDefault().Fecha;
                if (fecha.Value.AddHours(-3) > DateTime.Now)
                {
                    foreach (Quiniela q in quinielas)
                    {
                        var quinielaActual = db.Quinielas.Where(qn => qn.ID_Partido == q.ID_Partido)
                            .Where(qnl => qnl.ID_Usuario == q.ID_Usuario)
                            .FirstOrDefault();
                        quinielaActual.FechaMod = DateTime.Now;
                        quinielaActual.GolesEquipo1 = q.GolesEquipo1;
                        quinielaActual.GolesEquipo2 = q.GolesEquipo2;
                        q.Partido = db.Partidos.Where(p => p.ID_Partido == q.ID_Partido).FirstOrDefault();
                        if (q.GolesEquipo1 > q.GolesEquipo2)
                            quinielaActual.Ganador = db.Partidos.Where(p => p.ID_Partido == q.ID_Partido).FirstOrDefault().Equipo1;
                        else if (q.GolesEquipo2 > q.GolesEquipo1)
                            quinielaActual.Ganador = db.Partidos.Where(p => p.ID_Partido == q.ID_Partido).FirstOrDefault().Equipo2;
                    }
                    try
                    {
                        db.SaveChanges();
                        ViewBag.SaveMessage = "Datos guardados";
                        ViewBag.FailMessage = "";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.FailMessage = ex.Message;
                        ViewBag.SaveMessage = "";
                    }
                }
                else
                {
                    foreach (Quiniela q in quinielas)
                    {
                        q.Partido = db.Partidos.Where(p => p.ID_Partido == q.ID_Partido).FirstOrDefault();
                    }
                    ViewBag.FailMessage = "Esta ronda ya está cerrada.";
                    ViewBag.SaveMessage = "";
                }
                //TempData["DataUrl"] = "data-url=" + Url.Content("~") + "Quinielas/Rondas?ID_Grupo=" + quinielas[0].ID_Grupo + "&ID_Usuario=" + quinielas[0].ID_Usuario + "&token=" + Session["AccessToken"].ToString();
                //return RedirectToAction("Rondas", "Quinielas", new { ID_Grupo = quinielas[0].ID_Grupo,  ID_Usuario = quinielas[0].ID_Usuario, token = Session["AccessToken"].ToString() });
                //return RedirectToAction("Home", "Index");
                return View(quinielas);
            }
            return View(quinielas);
        }

        public ActionResult Ranking(int ID_Grupo, int ID_Usuario)
        {
            int idGrupo = ID_Grupo;

            //var ranking = from r in db.UsuarioGrupoes
            //              where r.ID_Grupo == idGrupo
            //              select r;
            ViewBag.ID_Usuario = ID_Usuario;
            var rankingView = from rv in db.RankingViews
                              where rv.ID_Grupo == 2
                              select rv;
            //return View(ranking.OrderByDescending(r=>r.TotalPuntos).ToList());
            return View(rankingView.OrderByDescending(r => r.TotalPuntos).ThenBy(rv => rv.Diferencia).ToList());
        }

        public ActionResult Nickname(int ID_Usuario)
        {
            var user = from u in db.Usuarios
                       where u.ID_Usuario == ID_Usuario
                       select u;
            return View(user.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult Nickname(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var user = from u in db.Usuarios
                           where u.ID_Usuario == usuario.ID_Usuario
                           select u;
                user.FirstOrDefault().Nickname = usuario.Nickname;
                db.SaveChanges();
                TempData["DataUrl"] = "data-url=" + Url.Content("~") + "Quiniela/Rondas";
                return RedirectToAction("Rondas", "Quiniela", new { ID_Grupo = 2, ID_Usuario = usuario.ID_Usuario, token = Session["AccessToken"].ToString() });
            }
            return View(usuario);
        }

        public ActionResult Detalles()
        {
            ViewBag.Message = "Mi Quiniela";
            List<Quiniela> quinielas = (List<Quiniela>)TempData["Quinielas"];
            return View(quinielas.ToList());
        }

        public ActionResult Usuarios(int ID_Grupo)
        {
            int idGrupo = ID_Grupo;

            var usuarios = from r in db.UsuarioGrupoes
                          where r.ID_Grupo == idGrupo
                          select r;
            return View(usuarios.OrderByDescending(r => r.TotalPuntos).ToList());
        }

        public ActionResult DetallesUsuario(int ID_Grupo, int ID_Usuario)
        {
            //int idUsuario = Int32.Parse(TempData["ID_Usuario"].ToString());
            int idUsuario = ID_Usuario;

            var quinielas = db.Quinielas.Where(q => q.ID_Usuario == idUsuario)
                            .Where(g => g.ID_Grupo == ID_Grupo);
            ViewBag.Title = db.Grupos.Where(g => g.ID_Grupo == ID_Grupo).FirstOrDefault().Nombre;
            ViewBag.Message = "Quiniela de " + db.Usuarios.Where(u=>u.ID_Usuario == idUsuario).FirstOrDefault().Nickname;
            return View(quinielas.ToList());
        }

        public ActionResult Reglas(int ID_Grupo, int ID_Usuario)
        {
            //int idUsuario = Int32.Parse(TempData["ID_Usuario"].ToString());
            int idUsuario = ID_Usuario;

            var quinielas = db.Quinielas.Where(q => q.ID_Usuario == idUsuario)
                            .Where(g => g.ID_Grupo == ID_Grupo);
            
            return View(quinielas.ToList());
        }

        public ActionResult Partidos(int ID_Grupo, int ID_Usuario)
        {
            TempData["ID_Grupo"] = ID_Grupo;
            TempData["ID_Usuario"] = ID_Usuario;
            ViewBag.Title = "Partidos";
            return View(db.Partidos.OrderBy(p=>p.Fecha).ToList());
        }

        [HttpPost]
        public ActionResult Partidos(List<Partido> Partidos)
        {
            try
            {
                TempData["ID_Grupo"] = Request.QueryString["ID_Grupo"];
                TempData["ID_Usuario"] = Request.QueryString["ID_Usuario"];
                ViewBag.Title = "Partidos";
                int i = 0;
                foreach (Partido p in Partidos)
                {
                    if (p.GolesEquipo1.HasValue && p.GolesEquipo2.HasValue)
                    {
                        var partido = (from par in db.Partidos
                                       where par.ID_Partido == p.ID_Partido
                                       select par).FirstOrDefault();
                        partido.GolesEquipo1 = p.GolesEquipo1;
                        partido.GolesEquipo2 = p.GolesEquipo2;
                        if (p.GolesEquipo1 > p.GolesEquipo2)
                            partido.Ganador = partido.Equipo1;
                        if (p.GolesEquipo1 < p.GolesEquipo2)
                            partido.Ganador = partido.Equipo2;

                        //Calcular puntos 
                        var quinielas = from q in db.Quinielas
                                        where q.ID_Partido == p.ID_Partido
                                        select q;
                        foreach (Quiniela q in quinielas)
                        {
                            var usuario = (from u in db.UsuarioGrupoes
                                           where u.ID_Usuario == q.ID_Usuario
                                           //&& u.ID_Grupo == q.ID_Grupo
                                           select u).FirstOrDefault();

                            //Reiniciar puntaje al comenzar a re-calcular
                            if (i == 0)
                                usuario.TotalPuntos = 0;

                            if (q.GolesEquipo1 == partido.GolesEquipo1)
                            {
                                if (q.GolesEquipo2 == partido.GolesEquipo2)
                                {
                                    usuario.TotalPuntos += 3;
                                }
                                else if (q.Ganador == partido.Ganador)
                                {
                                    usuario.TotalPuntos += 1;
                                }
                            }
                            else if (q.Ganador == partido.Ganador)
                            {
                                usuario.TotalPuntos += 1;
                            }
                        }
                        i++;
                    }
                }
                try
                {
                    db.SaveChanges();
                    ViewBag.SaveMessage = "Datos guardados";
                    ViewBag.FailMessage = "";
                }
                catch (Exception ex)
                {
                    ViewBag.FailMessage = ex.Message;
                    ViewBag.SaveMessage = "";
                }
            }
            catch (Exception excep)
            {
                ViewBag.FailMessage = excep.Message;
                ViewBag.SaveMessage = "";
            }
            return View(db.Partidos.OrderBy(p => p.Fecha).ToList());
        }
    }
}
