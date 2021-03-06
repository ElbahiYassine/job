﻿using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace JobFjib_Empty.Profil
{
    public partial class Candidats : System.Web.UI.Page
    {
        jobContextDataContext db = new jobContextDataContext();

        bool vil = false, pro = false, niv = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var villes = db.Candidat.Select(m => new { m.ville }).GroupBy(x => x.ville).Select(x => x.First()).ToList();

                dlVille.DataTextField = "ville";

                dlVille.DataSource = villes;
                dlVille.DataBind();
                dlVille.Items.Insert(0, new ListItem("Tout les Villes", "0"));


                var professions = db.Candidat.Select(m => new { m.Profession }).GroupBy(x => x.Profession).Select(x => x.First()).ToList();


                dlProf.DataTextField = "Profession";

                dlProf.DataSource = professions;
                dlProf.DataBind();
                dlProf.Items.Insert(0, new ListItem("Tout les Professions", "0"));


                var niveau = db.NiveauEtude.ToList();

                dlNiveau.DataTextField = "designation";
                dlNiveau.DataValueField = "IdNiveau";
                dlNiveau.DataSource = niveau;
                dlNiveau.DataBind();

                dlNiveau.Items.Insert(0, new ListItem("Tout les Niveaux", "0"));

                var can = from c in db.Candidat select new { c.candidatId, c.Profession, c.nom, c.photo, c.prenom, c.ville, Age = DateTime.Now.Year - c.dateNaissance.Year };

                repCandidats.DataSource = can;
                repCandidats.DataBind();
            }

        }

        protected void dlProf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlProf.SelectedIndex != 0)
            {
                pro = true;
            }
        }

        protected void dlNiveau_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlNiveau.SelectedIndex != 0)
            {
                niv = true;
            }
        }

        

        protected void dlVille_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dlVille.SelectedIndex != 0)
            {
                vil = true;
            }
        }

        protected void repCandidats_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "btnCan")
            {
                Session["IdCan"] = e.CommandArgument.ToString();
                Response.Redirect("~/Profil/candidat.aspx");
            }
        }

        //Button Filtré

        protected void Unnamed_ServerClick(object sender, EventArgs e)
        {
            

            var can = from c in db.Candidat select new { c.candidatId, c.Profession, c.NiveauEtude_IdNiveau , c.nom, c.photo, c.prenom, c.ville, Age = DateTime.Now.Year -  c.dateNaissance.Date.Year };


            if (pro && vil && niv)
            {
                can = from d in can
                      where d.NiveauEtude_IdNiveau == int.Parse(dlNiveau.SelectedValue)
                      join
                       v in can
                       on
                       d.candidatId equals v.candidatId
                      where v.ville == dlVille.SelectedItem.ToString()
                      join
                       c in can on
                       d.candidatId equals c.candidatId
                      where d.Profession == dlProf.SelectedItem.ToString()
                       select d;
            }
            else
            {
                if (!pro || !vil || !niv)
                {

                    if (pro == true) { can = can.Where(d => d.Profession == dlProf.SelectedItem.ToString()); pro = false; }

                    if (vil == true) { can = can.Where(p => p.ville == dlVille.SelectedItem.ToString()); vil = false; }

                    if (niv == true) { can = can.Where(c => c.NiveauEtude_IdNiveau == Convert.ToInt32(dlNiveau.SelectedValue)); niv = false; }
                }
            }
            if (can.ToList().Count == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "erreurAlert", "SearchInvalide()", true);
                Page_Load( sender,  e);
                return;


            }
            repCandidats.DataSource = can.ToList();
            repCandidats.DataBind();

            dlNiveau.SelectedIndex = 0;
            dlVille.SelectedIndex = 0;
            dlProf.SelectedIndex = 0;
        }

        
    }
}