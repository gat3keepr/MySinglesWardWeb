using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW;
using MSW.Model;
using System.IO;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Mail;
using MSW.Models;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{
    [HandleError]
	public class PhotoController : Controller
	{
		//
		// GET: /Photo/

		[Authorize]
		public ActionResult UploadPicture()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult UploadPicture(String empty)
		{
			try
			{
				HttpPostedFileBase file = Request.Files["picture"];

                //Check if the file uploaded is the file
                bool FileOk = _checkPhotoFile(file);

                if (FileOk)
                {
                    bool isStake = false;
                    _processNewPhoto(file, isStake); 

                    return RedirectToAction("Resize", "Photo");
                }
                else
                {
                    ViewData["Error"] = "Unsupported Picture File. Use .jpeg, .jpg, .png, .gif";
                    return View();
                }
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				ViewData["Error"] = "Please Email support@mysinglesward.com";
				return View();
			}
		}                     

		[Authorize]
		public ActionResult Resize()
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			Photo photo = Photo.getPhoto(user.MemberID);

            _setCropInformation(photo);		

			return View();
		}       

		[Authorize]
		[HttpPost]
		public ActionResult Resize(int X, int Y, int W, int H)
        {
			MSWUser user = MSWUser.getUser(User.Identity.Name);

            _ResizeImage(X, Y, W, H, user.MemberID);

			if (user.IsBishopric)
				Photo.Moderate(user.MemberID, true);

			TempData["PhotoUploaded"] = true;

            return RedirectToAction("Profile", "Home");
        }

		#region StakePhotos

		[Authorize(Roles = "StakePres,Stake")]
		public ActionResult UploadStakePicture()
		{
			return View();
		}

		[Authorize(Roles = "StakePres,Stake")]
		[HttpPost]
		public ActionResult UploadStakePicture(string empty)
		{
			try
			{
				HttpPostedFileBase file = Request.Files["picture"];

                //Check if the file uploaded is the file
                bool FileOk = _checkPhotoFile(file);

				if (FileOk)
				{
                    bool isStake = true;
                    _processNewPhoto(file, isStake);

					return RedirectToAction("StakeResize", "Photo");
				}
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				ViewData["Error"] = "Please Email support@mysinglesward.com";
				return View();
			}

			ViewData["Error"] = "Unsupported Picture File. Use .jpeg, .jpg, .png, .gif";
			return View();
		}

		[Authorize(Roles = "StakePres,Stake")]
		public ActionResult StakeResize()
		{
			StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
			StakePhoto picture = StakePhoto.getStakePhoto(user.MemberID);

            _setCropInformation(null, picture);	
			
			return View();
		}

		[Authorize(Roles = "StakePres,Stake")]
		[HttpPost]
		public ActionResult StakeResize(int X, int Y, int W, int H)
		{
			StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
            
            _ResizeImage(X, Y, W, H, null, user.MemberID); 

			return RedirectToAction("Index", "Stake");
		}

		#endregion

		#region BishopricPhotoHandlers
		[Authorize(Roles = "Bishopric, Clerk")]
		[HttpPost]
		public ActionResult UploadPictures(int uploadID)
		{
            //CONTROL - Make use user is in the ward of the bishopric user uploading photos
			MSWUser user = MSWUser.getUser(uploadID);
			if (user.WardStakeID != double.Parse(Session["WardStakeID"] as string))
				return RedirectToAction("Unauthorized", "Home");

			try
			{
				HttpPostedFileBase file = Request.Files["picture"];

                //Check if the file uploaded is the file
                bool FileOk = _checkPhotoFile(file);


				if (FileOk)
				{
					bool isStake = false;
                    _processNewPhoto(file, isStake, user.MemberID); 

					return RedirectToAction("BishopCrop", "Photo", new { @memberID = uploadID });
				}
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				TempData["Error"] = "Please Email support@mysinglesward.com";
				return RedirectToAction("ManageWardList", "Bishopric");
			}

			TempData["Error"] = "Unsupported Picture File. Use .jpeg, .jpg, .png, .gif";
			return RedirectToAction("ManageWardList", "Bishopric");
		}

		[Authorize(Roles = "Bishopric, Clerk")]
		public ActionResult BishopCrop(int memberID)
		{
			MSWUser user = MSWUser.getUser(memberID);
			if (user.WardStakeID != double.Parse(Session["WardStakeID"] as string))
				return RedirectToAction("Unauthorized", "Home");

			Photo photo = Photo.getPhoto(user.MemberID);

            _setCropInformation(photo);	

			ViewData["memberID"] = memberID;
			return View();
		}

		[Authorize(Roles = "Bishopric, Clerk")]
		[HttpPost]
		public ActionResult BishopCrop(int X, int Y, int W, int H, int memberID)
		{
			MSWUser user = MSWUser.getUser(memberID);
            if (user.WardStakeID != double.Parse(Session["WardStakeID"] as string))
                return RedirectToAction("Unauthorized", "Home");

            _ResizeImage(X, Y, W, H, user.MemberID);	
		
			//Photo uploaded by trusted source, Moderation Complete
			Photo.Moderate(user.MemberID, true);

			return RedirectToAction("ManageWardList", "Bishopric");
		}

        [Authorize(Roles = "Bishopric")]
        public bool ModeratePhoto(int memberID, bool isApproved)
        {
            MSWUser user = MSWUser.getUser(memberID);

            if (user.WardStakeID != double.Parse(Session["WardStakeID"] as string))
                throw new Exception();

			Photo.Moderate(memberID, isApproved);

            return true;
        }

		#endregion

		/*[Authorize(Roles = "Global")]
		public ActionResult FixPictures()
		{
			var db = new DBmsw();
			var pictures = db.t_Pictures;

			foreach (var picture in pictures)
			{
				// Specify a "currently active folder"
				string activeDir = Server.MapPath("");

				// Create a new file name. This example generates
				// a random string.
				string newFileName = "profile" + picture.ProfPicID.ToString();

				//Get File Extentsion
				string fileExtention = "";

				switch (picture.ContentType)
				{
					case "image/jpeg":
						fileExtention = ".jpg";
						break;
					case "image/pjpeg":
						fileExtention = ".jpg";
						break;
					case "image/pjpg":
						fileExtention = ".jpg";
						break;
					case "image/jpg":
						fileExtention = ".jpg";
						break;
					case "image/gif":
						fileExtention = ".jpg";
						break;
					case "image/x-png":
						fileExtention = ".jpg";
						break;
					case "image/png":
						fileExtention = ".jpg";
						break;
					default:
						MSWtools._sendException(new Exception("Bad Image Data " + picture.ContentType));
						break;
				}

				newFileName += fileExtention;

				// Combine the new file name with the path
				activeDir = System.IO.Path.Combine(activeDir, newFileName);

				// Create the file and write to it.
				// DANGER: System.IO.File.Create will overwrite the file
				// if it already exists. This can occur even with
				// random file names.
				if (!System.IO.File.Exists(activeDir))
				{
					using (System.IO.FileStream fs = System.IO.File.Create(activeDir))
					{
						fs.Write(picture.PictureData.ToArray(), 0, picture.PictureData.Length);
					}
				}

				var newPicture = new tPicture();
				newPicture.MemberID = picture.ProfPicID;
				newPicture.FileName = newFileName;
				newPicture.Stat = bool.Parse(picture.beenCropped.ToString());
				db.tPictures.InsertOnSubmit(newPicture);
			}
			db.SubmitChanges();
			return RedirectToAction("UnlockUser", "Global", null);
		}*/



		/*[Authorize(Roles = "Global")]
		public ActionResult FixStakePictures()
		{
			var db = new DBmsw();
			//var pictures = db.tStakePictures;

			foreach (var picture in pictures)
			{
				// Specify a "currently active folder"
				string activeDir = Server.MapPath("");

				// Create a new file name. This example generates
				// a random string.
				string newFileName = "stake" + picture.ProfPicID.ToString();

				//Get File Extentsion
				string fileExtention = "";

				switch (picture.ContentType)
				{
					case "image/jpeg":
						fileExtention = ".jpg";
						break;
					case "image/pjpeg":
						fileExtention = ".jpg";
						break;
					case "image/pjpg":
						fileExtention = ".jpg";
						break;
					case "image/jpg":
						fileExtention = ".jpg";
						break;
					case "image/gif":
						fileExtention = ".jpg";
						break;
					case "image/x-png":
						fileExtention = ".jpg";
						break;
					case "image/png":
						fileExtention = ".jpg";
						break;
					default:
						MSWtools._sendException(new Exception("Bad Image Data " + picture.ContentType));
						break;
				}

				newFileName += fileExtention;

				// Combine the new file name with the path
				activeDir = System.IO.Path.Combine(activeDir, newFileName);

				// Create the file and write to it.
				// DANGER: System.IO.File.Create will overwrite the file
				// if it already exists. This can occur even with
				// random file names.
				if (!System.IO.File.Exists(activeDir))
				{
					using (System.IO.FileStream fs = System.IO.File.Create(activeDir))
					{
						fs.Write(picture.PictureData.ToArray(), 0, picture.PictureData.Length);
					}
				}

				var newPicture = new tStakePhoto();
				newPicture.MemberID = picture.ProfPicID;
				newPicture.FileName = newFileName;
				newPicture.Cropped = bool.Parse(picture.beenCropped.ToString());
				db.tStakePhotos.InsertOnSubmit(newPicture);
			}
			db.SubmitChanges();
			return RedirectToAction("UnlockUser", "Global", null);
		}*/

		/*
		 *  Writes original photo file for Bishopric and Member Users
		 */ 
		private string _WriteFile(int MemberID, string contentType, byte[] pictureData, string serverFilePath = null, int? width = null, int? height = null)
		{
			// Specify a "currently active folder"
			string activeDir = serverFilePath == "" ? Server.MapPath("") : serverFilePath;
            string dir = serverFilePath == "" ? Server.MapPath("") : serverFilePath;

			// Create a new file name. This example generates
			// a random string.
			string newFileName = "profile" + MemberID.ToString();

			//Get File Extentsion
			string fileExtention = "";

			switch (contentType)
			{
				case "image/jpeg":
					fileExtention = ".jpg";
					break;
				case "image/pjpeg":
					fileExtention = ".jpg";
					break;
				case "image/pjpg":
					fileExtention = ".jpg";
					break;
				case "image/jpg":
					fileExtention = ".jpg";
					break;
				case "image/gif":
					fileExtention = ".jpg";
					break;
				case "image/x-png":
					fileExtention = ".jpg";
					break;
				case "image/png":
					fileExtention = ".jpg";
					break;
				default:
					MSWtools._sendException(new Exception("Bad Image Data " + contentType));
					break;
			}

			newFileName = newFileName + '-' + DateTime.Now.Ticks.ToString() + fileExtention;

			// Combine the new file name with the path
			activeDir = System.IO.Path.Combine(activeDir, newFileName);

			// Create the file and write to it.
			// DANGER: System.IO.File.Create will overwrite the file
			// if it already exists. This can occur even with
			// random file names.
			using (System.IO.FileStream fs = System.IO.File.Create(activeDir))
			{
				fs.Write(pictureData, 0, pictureData.Length);
			}

			//Process Photo that has been written. Massive files will extend outside the screen and make cropping impossible
			string destFileName = "profile" + MemberID.ToString() + '-' + (DateTime.Now.Ticks + 1).ToString() + fileExtention;
			newFileName = _processImage(dir, newFileName, destFileName, width, height);

			return newFileName;
		}

		/*
		 * Writes Photo file after cropping occurs
		 */
		private string _WriteFile(int MemberID, byte[] pictureData, string FileName)
		{
			using (System.IO.FileStream fs = System.IO.File.Create(Server.MapPath("") + "\\" + FileName))
			{
				fs.Write(pictureData, 0, pictureData.Length);
			}

			return FileName;
		}

		/*
		 *  Writes original photo file for Stake Users
		 */ 
		private string _WriteStakeFile(int MemberID, string contentType, byte[] pictureData)
		{
			// Specify a "currently active folder"
			string activeDir = Server.MapPath("");

			// Create a new file name. This example generates
			// a random string.
			string newFileName = "stake" + MemberID.ToString();

			//Get File Extentsion
			string fileExtention = "";

			switch (contentType)
			{
				case "image/jpeg":
					fileExtention = ".jpg";
					break;
				case "image/pjpeg":
					fileExtention = ".jpg";
					break;
				case "image/pjpg":
					fileExtention = ".jpg";
					break;
				case "image/jpg":
					fileExtention = ".jpg";
					break;
				case "image/gif":
					fileExtention = ".jpg";
					break;
				case "image/x-png":
					fileExtention = ".jpg";
					break;
				case "image/png":
					fileExtention = ".jpg";
					break;
				default:
					MSWtools._sendException(new Exception("Bad Image Data " + contentType));
					break;
			}

			newFileName = newFileName + '-' + DateTime.Now.Ticks.ToString() + fileExtention;

			// Combine the new file name with the path
			activeDir = System.IO.Path.Combine(activeDir, newFileName);

			// Create the file and write to it.
			// DANGER: System.IO.File.Create will overwrite the file
			// if it already exists. This can occur even with
			// random file names.
			using (System.IO.FileStream fs = System.IO.File.Create(activeDir))
			{
				fs.Write(pictureData, 0, pictureData.Length);
			}

			//Process Photo that has been written. Massive files will extend outside the screen and make cropping impossible
			string destFileName = "stake" + MemberID.ToString() + '-' + (DateTime.Now.Ticks + 1).ToString() + fileExtention;
			newFileName = _processImage(Server.MapPath(""), newFileName, destFileName);

			return newFileName;
		}

		private void _DeleteFile(string fileName, string serverFilePath = null)
		{
			if (fileName == "profile-1.jpg" || fileName == "stake-1.jpg")
				return;
            string path = serverFilePath != "" ? serverFilePath + "\\" + fileName : Server.MapPath("") + "\\" + fileName;
            try
            {
				FileInfo file = new FileInfo(path);
				file.Delete();
            }
            catch (Exception e)
            {
                MSWtools._sendException(e);
            }
		}

		private byte[] Crop(string filename, int Width, int Height, int X, int Y)
		{
			try
			{
				using (Image OriginalImage = Image.FromFile(Server.MapPath("") + "\\" + filename))
				{
					using (Bitmap bmp = new Bitmap(Width, Height))
					{
						bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
						using (Graphics Graphic = Graphics.FromImage(bmp))
						{
							Graphic.SmoothingMode = SmoothingMode.AntiAlias;
							Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
							Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
							Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, Width, Height), X, Y, Width, Height, GraphicsUnit.Pixel);
							MemoryStream ms = new MemoryStream();
							bmp.Save(ms, OriginalImage.RawFormat);
							return ms.GetBuffer();
						}
					}
				}
			}
			catch (Exception Ex)
			{
				throw (Ex);
			}
		}

		private byte[] _Resize(Stream Img, int Width, int Height, int X, int Y)
		{
			try
			{
				using (Image OriginalImage = Image.FromStream(Img))
				{
					using (Bitmap bmp = new Bitmap(200, 200))
					{
						bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
						using (Graphics Graphic = Graphics.FromImage(bmp))
						{
							Graphic.SmoothingMode = SmoothingMode.AntiAlias;
							Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
							Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
							Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, 200, 200), 0, 0, Width, Height, GraphicsUnit.Pixel);
							MemoryStream ms = new MemoryStream();
							bmp.Save(ms, OriginalImage.RawFormat);
							return ms.GetBuffer();
						}
					}
				}
			}
			catch (Exception Ex)
			{
				throw (Ex);
			}
		}


		//Process Photo that has been written. Massive files will extend outside the screen and make cropping impossible
		private string _processImage(string dir, string filename, string destFileName, int? width = null, int? height = null)
		{
			string source = System.IO.Path.Combine(dir, filename);
			string destination = System.IO.Path.Combine(dir, destFileName);
			Image image = Image.FromFile(source);
			int WIDTH = width == null ? 819 : (int) width;
			int HEIGHT = height == null ? 614 : (int) height;

			//Checks whether the image is portrait or landscape 
			bool portrait = image.Height > image.Width;

			//Checks to see if the image needs to be resized
			bool resized = (image.Width > WIDTH && !portrait) || ((image.Width > HEIGHT || image.Height > WIDTH) && portrait);

			image.Dispose();

			//Resize image based on portait or landscape			
			if(portrait && resized)
				ImageProcessor.ResizeImage(source, destination, HEIGHT, WIDTH);
			else if(resized)
				ImageProcessor.ResizeImage(source, destination, WIDTH, HEIGHT);

			return resized ? destFileName : filename;
		}

		private void _NewSession()
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);

			if (user != null)
			{
				//Checks the WardID and changes the ID to zero if the stake is missing
				//user = MSWtools._checkWardID(user);

				Session["Username"] = user.UserName;
				Session["WardStakeID"] = user.WardStakeID.ToString();
				Session["MemberID"] = user.MemberID.ToString();
				Session["IsBishopric"] = user.IsBishopric.ToString();
			}
			else
			{
				StakeUser stakeUser = StakeUser.getStakeUser(User.Identity.Name);

				//Checks the WardID and changes the ID to zero if the stake is missing
				stakeUser = MSWtools._checkStakeID(stakeUser);

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

        private bool _checkPhotoFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                ViewData["Error"] = "Please try uploading your photo again or try another photo.";
                return false;
            }

            //Check File Extention
            String[] allowedExtensions = { "image/jpeg", "image/pjpeg", "image/pjpg", "image/jpg", "image/gif", "image/x-png", "image/png" };
            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                try
                {
                    if (file.ContentType.Equals(allowedExtensions[i]))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            //Photo was not a valid extention
            return false;
        }

        internal void _processNewPhoto(HttpPostedFileBase file, bool isStake, int? MemberID = null, string serverFilePath = "")
        {
            String ContentType = file.ContentType;

            Int32 length = file.ContentLength;
            byte[] image = new byte[length];
            file.InputStream.Read(image, 0, length);

            if (!isStake)
            {
                //If a bishopric user is uploading a photo for a member, MemberID will not be null
                MSWUser user = null;
                if(MemberID == null)
                    user = MSWUser.getUser(User.Identity.Name);
                else
                    user = MSWUser.getUser((int)MemberID);

                //Delete old photo if member changes what photo they want moderated
                Photo photo = Photo.getPhoto(user.MemberID);
                if (photo != null)
                    if (photo.NewPhotoFileName != null)
                        _DeleteFile(photo.NewPhotoFileName, serverFilePath);

                //Save New Photo information to be cropped
                Photo.saveNewPhoto(user.MemberID, _WriteFile(user.MemberID, ContentType, image, serverFilePath));
            }
            else
            {
                StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
                StakePhoto oldPhoto = StakePhoto.getStakePhoto(user.MemberID);

                //delete old photo
                if (oldPhoto != null)
                    _DeleteFile(oldPhoto.FileName);

                StakePhoto.saveStakePhoto(user.MemberID, _WriteStakeFile(user.MemberID, ContentType, image));
            }
            
        }

        //Sets crop information for jCrop on the view
        private void _setCropInformation(Photo photo = null, StakePhoto stakePhoto = null)
        {
            if (photo != null) //Bishopric or Member Photo
            {
                ViewData["Image"] = photo.NewPhotoFileName;

                Image image = Image.FromFile(Server.MapPath("") + "\\" + photo.NewPhotoFileName);

                ViewData["Height"] = (int)image.PhysicalDimension.Height;
                ViewData["Width"] = (int)image.PhysicalDimension.Width;
                image.Dispose();
            }
            else //Stake Photo
            {
                ViewData["Image"] = stakePhoto.croppingFileName;

                Image image = Image.FromFile(Server.MapPath("") + "\\" + stakePhoto.croppingFileName);

                ViewData["Height"] = (int)image.PhysicalDimension.Height;
                ViewData["Width"] = (int)image.PhysicalDimension.Width;
                image.Dispose();
            }
        }

        //Used for final Crop
        private void _ResizeImage(int X, int Y, int W, int H, int? MemberID = null, int? StakeID = null)
        {
			//Member Resize
            if (MemberID != null)
            {
                Photo picture = Photo.getPhoto((int)MemberID);
                string originalFileName = picture.NewPhotoFileName;

                byte[] newImage = Crop(picture.NewPhotoFileName, W, H, X, Y);

                MemoryStream MemStream = new MemoryStream();
                MemStream.Write(newImage, 0, newImage.Length);
                newImage = _Resize(MemStream, W, H, X, Y);

                string FileName = "profile" + MemberID + '-' + DateTime.Now.Ticks.ToString() + '.' + picture.NewPhotoFileName.Split('.')[1];
                _WriteFile((int)MemberID, newImage, FileName);
                Photo.cropPhoto((int)MemberID, FileName, true);

                //delete old photo
                _DeleteFile(originalFileName);

            }
            else //Stake Resize
            {
                StakePhoto picture = StakePhoto.getStakePhoto((int)StakeID);

                string fileName = picture.croppingFileName;

                byte[] newImage = Crop(picture.croppingFileName, W, H, X, Y);

                MemoryStream MemStream = new MemoryStream();
                MemStream.Write(newImage, 0, newImage.Length);
                newImage = _Resize(MemStream, W, H, X, Y);

                picture.FileName = "stake" + StakeID + '-' + DateTime.Now.Ticks.ToString() + '.' + picture.FileName.Split('.')[1];
                _WriteFile((int)StakeID, newImage, picture.FileName);
                StakePhoto.cropStakePhoto((int)StakeID, picture.FileName, true);

                //delete old photo
                _DeleteFile(fileName);
            }
        }

        internal void _processPhotoFromApp(HttpPostedFileBase file, bool isStake, int MemberID, string serverFilePath = null)
        {
            String ContentType = file.ContentType;

            Int32 length = file.ContentLength;
            byte[] image = new byte[length];
            file.InputStream.Read(image, 0, length);

            if (!isStake)
            {
                //If a bishopric user is uploading a photo for a member, MemberID will not be null
                MSWUser user = null;
                if (MemberID == null)
                    user = MSWUser.getUser(User.Identity.Name);
                else
                    user = MSWUser.getUser((int)MemberID);

                //Delete old photo if member changes what photo they want moderated
                Photo photo = Photo.getPhoto(user.MemberID);
                if (photo != null)
                    if (photo.NewPhotoFileName != null)
                        _DeleteFile(photo.NewPhotoFileName, serverFilePath);

                //Save New Photo information
                string filename = _WriteFile(user.MemberID, ContentType, image, serverFilePath, 200, 200);
                Photo.saveNewPhoto(user.MemberID, filename);
                Photo.cropPhoto(user.MemberID, filename, true);
            }
            else
            {
                StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
                StakePhoto oldPhoto = StakePhoto.getStakePhoto(user.MemberID);

                //delete old photo
                if (oldPhoto != null)
                    _DeleteFile(oldPhoto.FileName);

                StakePhoto.saveStakePhoto(user.MemberID, _WriteStakeFile(user.MemberID, ContentType, image));
            }
        }
    }
}
