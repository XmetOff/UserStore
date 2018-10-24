using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class UserController : Controller
    {
        private UsersDbContext db = new UsersDbContext();
        public int pageSize = 5;


        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //[HttpPost]
        //public ActionResult Index(List<User> users)
        //{
        //    return View(users);
        //}


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Year,Login,Password,About,Adress")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Year,Login,Password,About,Adress")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IEnumerable<User> Search(string query)
        {
            IEnumerable<User> result;
            if (query == "" || query == null)
            {
                result = db.Users;
            }
            else
            {
                result = from i in db.Users
                         where i.FirstName.Contains(query) || i.LastName.Contains(query) || i.Adress.Contains(query) ||
                             i.Login.Contains(query) || i.About.Contains(query) || i.Year.ToString().Contains(query)
                         select i;
            }

            return result;
        }

        [HttpPost]
        public ActionResult SearchResult(string query, int page)
        {
            List<User> users = Search(query).ToList();
            IEnumerable<User> usersPerPages = users.Skip((page - 1) * pageSize).Take(pageSize);
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = users.Count };
            IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Users = usersPerPages };
            return PartialView(ivm);
        }



        [HttpPost]
        public ActionResult ExportExcel(string queryExcel)
        {
            IEnumerable<User> result = Search(queryExcel);


            var users = result.ToList();

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("UsersSheet");


            for (int i = 0; i < users.Count; i++)
            {

                var r = sheet.CreateRow(i);

                r.CreateCell(0).SetCellValue(users[i].FirstName);
                r.CreateCell(1).SetCellValue(users[i].LastName);
                r.CreateCell(2).SetCellValue(users[i].Login);
                r.CreateCell(3).SetCellValue(users[i].Password);
                r.CreateCell(4).SetCellValue(users[i].Year);
                r.CreateCell(5).SetCellValue(users[i].About);
                r.CreateCell(6).SetCellValue(users[i].Adress);

            }
            using (MemoryStream exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
                Response.Charset = Encoding.GetEncoding("ISO-8859-1").EncodingName;
                Response.ContentType = "application/vnd.ms-excel"; //xls
                                                                   // For xlsx, use: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                Response.AddHeader("content-disposition", String.Format("attachment; filename={0}.xls", "Users"));
                Response.Clear();
                Response.BinaryWrite(exportData.GetBuffer());
                Response.End();
            }

            return View("Success");

        }


        ////[HttpPost]
        ////public ActionResult SearchResult(List<User> users)
        ////{
        ////    return View(users);
        ////}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}