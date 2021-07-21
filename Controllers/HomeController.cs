using CsvHelper;
using IAccessSolutions.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace IAccessSolutions.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CSV()
        {
            return View();
        }

        public ActionResult ListData()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert_Data(HttpPostedFileBase postedFile)
        {
            var CSV_Extention = ConfigurationManager.AppSettings["CSV_Extention"].ToString();
            var Result_List_CSV = new List<CSVEntities>();
           

            if (postedFile != null)
            {
                try
                {
                    string File_Extension = Path.GetExtension(postedFile.FileName);
                    string File_Name = Path.GetFileName(postedFile.FileName);
                    string File_Path = Server.MapPath("~/Uploads/" + File_Name);
                    postedFile.SaveAs(File_Path);

                    if (File_Extension != CSV_Extention)
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        RedirectToAction("CSV");
                    }

                    using (var sreader = new StreamReader(postedFile.InputStream))
                    {

                        string[] headers = sreader.ReadLine().Split(',');

                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            char[] charsToTrim = { '"', '"' };

                            var Obj_String_ID = rows[0].ToString().Trim(charsToTrim);
                            var Obj_String_Content = rows[1].ToString().Replace(" ", "").TrimEnd(charsToTrim);

                            //var rx = new Regex("Keywords", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            //Match match = rx.Match(Obj_String_Content);

                            Regex rx = new Regex("Keywords", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            MatchCollection matches = rx.Matches(Obj_String_Content);

                            Result_List_CSV.Add(new CSVEntities
                            {
                                String_ID = Obj_String_ID,
                                String_Content = Obj_String_Content,
                                Match_Times = matches.Count
                            });

                        }
                    }

                    Result_List_CSV.Count();

                    return View("CSV", Result_List_CSV);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
                finally
                {

                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Search_Data(string Obj_Post)
        {
            var Result_List_CSV = new List<CSVEntities>();

            try
            {

                var Models = new CSVEntities
                {
                    String_Content = Obj_Post.ToString()
                };


                return View("CSV");

            }
            catch (Exception Error)
            {
                ViewBag.Message = Error.Message;

            }
            finally
            { 
            
            }
            return View();
        }

        public ActionResult Create_CSV_File()
        {
            try
            {
                string csv = string.Empty;
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=File Test.csv");
                Response.Charset = "";
                Response.ContentType = "application/excel";
                Response.Output.Write(csv);
                Response.Flush();
                Response.End();
                return View("CSV");
            }
            catch (Exception Error)
            {
                ViewBag.Message = "Error";
                Error.Message.ToString();
                throw;
            }
            finally
            { 
            
            }
        }
    }
}