using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MSW.Model;
using MSW;
using System.Net.Mail;
using System.Text;
using MSW.Models;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{
    [HandleError]
    public class PrintController : Controller
    {
        //
        // GET: /Print/
        #region Bishopric & Ward Level
        [Authorize(Roles = "Activities, Bishopric, Clerk, Elders Quorum,Emergency,"
            + "Employment,FHE,Institute,Mission,Music,Relief Society, Teaching,Temple/FamHist")]
        public ActionResult GetData()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!_checkValidWard())
                return RedirectToAction("NotInWard", "Home");

            SelectListItem[] printList = new SelectListItem[3];
            printList[0] = new SelectListItem { Text = "Print by Last Name", Value = "lastName" };
            printList[2] = new SelectListItem { Text = "Print by Apartment", Value = "apartment" };
            printList[1] = new SelectListItem { Text = "Print by First Name & M/F", Value = "name" };
            ViewData["DropDown"] = printList;

            ViewData["WardInfo"] = WardInfo.get(double.Parse(Session["WardStakeID"] as string));

            return View();
        }

        [Authorize(Roles = "Activities, Bishopric, Clerk, Elders Quorum,Emergency,"
            + "Employment,FHE,Institute,Member,Mission,Music,Relief Society, Teaching,Temple/FamHist")]
        public FileResult WardList(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<BishopricModel> bishopricUsers = _getBishopricUsers(double.Parse(Session["WardStakeID"] as string));
            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\WardList-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            doc.SetMargins(30, 10, 45, 30);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();
                PdfPTable bishopric = new PdfPTable(1);
                PdfPTable members = new PdfPTable(2);
                foreach (var user in bishopricUsers)
                {
                    GeneratePDF._AddBishopricMember(user, bishopric);
                }

                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddWardMember(user, members);
                }
                //BUG - last member doesnt print unless a blank box is added
                if (users.Count % 2 == 1)
                {
                    var blank = new Phrase();
                    PdfPCell nestedHousing = new PdfPCell(blank);
                    nestedHousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
                    members.AddCell(nestedHousing);
                }

                doc.Add(bishopric);
                doc.Add(new Chunk(Chunk.NEXTPAGE));
                doc.Add(members);
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Ward List Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
			mem = new FileStream(key, FileMode.Open);
			file = new FileStreamResult(mem, "application/pdf");

			return file;
        }

        private List<BishopricModel> _getBishopricUsers(double WardStakeID)
        {
            using (var db = new DBmsw())
            {
                List<BishopricModel> UserList = new List<BishopricModel>();
                try
                {
					Repository r = Repository.getInstance();
                    UserList = Cache.GetList(r.BishopricMembersID(WardStakeID), x => Cache.getCacheKey<BishopricModel>(x), y => BishopricModel.get(y));
                    UserList = UserList.OrderBy(x => x.data.BishopricCalling).ToList();
                }
                catch (Exception e)
                {
                    _sendException(e);
                }

                return UserList;
            }
        }

        private List<MemberModel> _getUsers(string printSelect, double WardStakeID, string AuxType)
        {
            if (printSelect == "" || printSelect == null)
                printSelect = " ";
            if (AuxType == null)
                AuxType = "all";

            using (var db = new DBmsw())
            {
                List<MemberModel> UserList = new List<MemberModel>();
                try
                {
					Repository r = Repository.getInstance();
                    UserList = Cache.GetList(r.PrintKeys(WardStakeID, AuxType), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));

                    if (printSelect.Equals("name"))
                        UserList = UserList.OrderBy(x => x.memberSurvey.gender).ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();
                    else if (printSelect.Equals("lastName"))
                        UserList = UserList.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
                    else if (printSelect.Equals("apartment"))
                        UserList = UserList.OrderBy(x => x.memberSurvey.residence).ThenBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
                    else
                        UserList = UserList.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
                    return UserList;


                }
                catch (Exception e)
                {
                    _sendException(e);
                    return UserList;
                }
            }
        }

        [Authorize(Roles = "Bishopric")]
        [HttpGet]
        public ActionResult BishopricPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Bishopric-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            doc.SetMargins(30, 10, 45, 30);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddBishopricPage(user, doc);
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Bishopric Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);
                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
			mem = new FileStream(key, FileMode.Open);
			file = new FileStreamResult(mem, "application/pdf");

			return file;
        }

        [Authorize(Roles = "Bishopric")]
        [HttpGet]
        public ActionResult BishopricCSV(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            String file = GenerateCSV.MakeBishopricFile(users);
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
            fileStream.FileDownloadName = "BishopricPrintOut.csv";
            return fileStream;

        }

        [Authorize(Roles = "Bishopric,Elders Quorum")]
        [HttpPost]
        public ActionResult EQPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), "elders");

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\EQ-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddAuxInfo(user, doc);
                    doc.Add(new Chunk(Chunk.NEXTPAGE));
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Elders Quorum Pdf");

                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Elders Quorum")]
        [HttpGet]
        public ActionResult EQCSV(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), "elders");

            String file = GenerateCSV.MakeAuxFile(users);
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
            fileStream.FileDownloadName = "eqPrintOut.csv";
            return fileStream;

        }

        [Authorize(Roles = "Bishopric,Relief Society")]
        [HttpPost]
        public ActionResult RSPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), "sisters");
            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\RS-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddAuxInfo(user, doc);
                    doc.Add(new Chunk(Chunk.NEXTPAGE));
                }

            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Relief Society Pdf");

                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }

            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Relief Society")]
        [HttpGet]
        public ActionResult RSCSV(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), "sisters");

            String file = GenerateCSV.MakeAuxFile(users);
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
            fileStream.FileDownloadName = "rsPrintOut.csv";
            return fileStream;

        }

        [Authorize(Roles = "Bishopric,Activities")]
        [HttpPost]
        public ActionResult ActivitiesPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Act-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddActivitesInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Activities Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Emergency")]
        [HttpPost]
        public ActionResult EmergencyPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Emergency-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddEmergencyInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Emergency Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Employment")]
        [HttpPost]
        public ActionResult EmploymentPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Employment-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddEmploymentInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Employment Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");
            return file;
        }

        [Authorize(Roles = "Bishopric,Temple/FamHist")]
        [HttpPost]
        public ActionResult FamHistPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\FamHist-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddFamHistInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Temple/Family Histoy Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,FHE")]
        [HttpPost]
        public ActionResult FHEPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect, double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\FHE-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddFHEInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("FHE Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Institute")]
        [HttpPost]
        public ActionResult InstitutePrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect,
                double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Institute-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddInstituteInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Institute Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Mission")]
        [HttpPost]
        public ActionResult MissionPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect,
                double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Mission-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();


                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddAuxInfo(user, doc);

                    doc.Add(new Chunk(Chunk.NEXTPAGE));

                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Mission Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Mission")]
        [HttpGet]
        public ActionResult MissionCSV(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect,
                double.Parse(Session["WardStakeID"] as string), null);

            String file = GenerateCSV.MakeAuxFile(users);
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
            fileStream.FileDownloadName = "missionPrintOut.csv";
            return fileStream;

        }

        [Authorize(Roles = "Bishopric,Music")]
        [HttpPost]
        public ActionResult MusicPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect,
                double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Music-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddMusicInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Music Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Teaching")]
        [HttpPost]
        public ActionResult TeachingPrint(String PrintSelect)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers(PrintSelect,
                double.Parse(Session["WardStakeID"] as string), null);

            string WardID = Session["WardStakeID"] as string;
            string key = Server.MapPath("") + "\\Teaching-" + PrintSelect + WardID;

            FileStreamResult file = null;
            FileStream mem = null;

            try
            {
                mem = new FileStream(key, FileMode.Open);
                file = new FileStreamResult(mem, "application/pdf");
                return file;
            }
            catch
            {
                // Create output PDF
                mem = new FileStream(key, FileMode.Create);
            }

            Document doc = new Document(PageSize.A4);
            try
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                doc.Open();

                int position = 0;
                foreach (MemberModel user in users)
                {
                    GeneratePDF.AddTeachingInfo(user, doc);
                    position++;
                    if (position == 4)
                    {
                        position = 0;
                        doc.Add(new Chunk(Chunk.NEXTPAGE));
                    }
                }
            }
            catch (DocumentException dex)
            {
                _sendException(dex);
                Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                _sendException(ioex);
                Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                _sendException(ex);
                Response.Write(ex.Message);
            }
            finally
            {
                try
                {
                    doc.AddAuthor("MyStudentWard.com");
                    doc.AddCreationDate();
                    doc.AddTitle("Teaching Pdf");
                    doc.Close();
                }
                catch (Exception e)
                {
                    _sendException(e);
                    doc = new Document(PageSize.A4);

                    PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                    doc.Open();
                    doc.Add(new Paragraph("There were no users that fit this area."));
                    doc.Close();
                }
            }
            mem = new FileStream(key, FileMode.Open);
            file = new FileStreamResult(mem, "application/pdf");

            return file;
        }

        [Authorize(Roles = "Bishopric,Clerk")]
        [HttpPost]
        public ActionResult ClerkCSV(string City, string State, int Zipcode)
        {
            if (Session["Username"] == null)
                _NewSession();

            List<MemberModel> users = _getUsers("lastname", double.Parse(Session["WardStakeID"] as string), null);

            //Create/Save a wardinfo object to use in report
            WardInfo wardInfo = WardInfo.get(double.Parse(Session["WardStakeID"] as string));
            if (wardInfo == null)
            {
                var wardInfoDBO = new tWardInfo();
                wardInfoDBO.WardID = double.Parse(Session["WardStakeID"] as string);
                wardInfoDBO.City = City;
                wardInfoDBO.State = State;
                wardInfoDBO.Zipcode = Zipcode;
                WardInfo.create(wardInfoDBO);
            }
            else
            {
                wardInfo.City = City;
                wardInfo.State = State;
                wardInfo.Zipcode = Zipcode;
                WardInfo.save(wardInfo);
            }

            //Encode CSV file and return to user
            String file = GenerateCSV.MakeClerkFile(users, double.Parse(Session["WardStakeID"] as string));
            byte[] byteArray = Encoding.ASCII.GetBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
            fileStream.FileDownloadName = "Multiple Record Request.csv";
            return fileStream;
        }
        #endregion

        #region Stake Level

        [Authorize(Roles = "StakePres, Stake")]
        public ActionResult GetStakeData()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!_checkValidStake())
                return RedirectToAction("NotInStake", "Stake");

            StakeWardModel thisStake = new StakeWardModel(double.Parse(Session["StakeID"] as string));

            return View(thisStake);
        }

        [Authorize(Roles = "StakePres, Stake")]
        [HttpGet]
        public ActionResult StakeCSV(double WardStakeID)
        {
            if (Session["Username"] == null)
                _NewSession();

			using (var db = new DBmsw())
			{
				List<tUser> users = new List<tUser>();

				if (WardStakeID == -1)
				{
					double stakeID = double.Parse(Session["StakeID"] as string);
					var wards = db.tWardStakes.Where(x => x.StakeID == stakeID);

					foreach (var ward in wards)
					{
						System.Linq.IQueryable<MSW.Model.tUser> moreUsers = (from user in db.tUsers
																			 where user.WardStakeID == ward.WardStakeID
																			 where user.IsBishopric == false
																			 orderby user.LastName
																			 select user);

						users.AddRange(moreUsers);
					}
				}
				else
				{
					var moreUsers = (from user in db.tUsers
									 where user.WardStakeID == WardStakeID
									 where user.IsBishopric == false
									 orderby user.LastName
									 select user);
					users.AddRange(moreUsers);
				}

				List<MemberModel> members = new List<MemberModel>();
				foreach (var user in users)
				{
					members.Add(MemberModel.get(user.MemberID));
				}
				members = members.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
				String file;
				if (User.IsInRole("StakePres"))
					file = GenerateCSV.MakeStakePresFile(members);
				else
					file = GenerateCSV.MakeStakeFile(members);
				byte[] byteArray = Encoding.ASCII.GetBytes(file);
				MemoryStream stream = new MemoryStream(byteArray);

				FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
				fileStream.FileDownloadName = "StakePrintOut.csv";
				return fileStream;
			}

        }

        #endregion

        private void _sendException(Exception e)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress("gatekeeper@mysinglesward.com", "Porter Hoskins"));
            message.From = new MailAddress("no-reply@mysinglesward.com", "Customer Service");
            message.Subject = "Error";
            message.IsBodyHtml = true;
            message.Body = "Here is the exception:\n" + e.Message + e.StackTrace;

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Send(message);
        }

        private void _NewSession()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            if (user != null)
            {
                Session["Username"] = user.UserName;
                Session["WardStakeID"] = user.WardStakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsBishopric"] = user.IsBishopric.ToString();
            }
            else
            {

                StakeUser stakeUser = StakeUser.getStakeUser(User.Identity.Name);
                
                Session["Username"] = stakeUser.UserName.ToString();
                Session["StakeID"] = stakeUser.StakeID.ToString();
                Session["MemberID"] = stakeUser.MemberID.ToString();
                Session["IsPresidency"] = stakeUser.IsPresidency.ToString();
                Session["HasPic"] = stakeUser.HasPic.ToString();
            }
        }



        private bool _checkValidWard()
        {
            if (int.Parse(Session["WardStakeID"] as string) == 0)
                return false;
            return true;
        }

        private bool _checkValidStake()
        {
            if (int.Parse(Session["StakeID"] as string) == 0)
                return false;
            return true;
        }
    }
}