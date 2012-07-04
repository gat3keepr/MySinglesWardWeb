using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	public enum PhotoStatus { NONE, UPLOADED, CROPPED, MODERATED }

	[Serializable]
	public class Photo
	{
		public int MemberID { get; set; }
		public string FileName { get; set; }
		public string NewPhotoFileName { get; set; }
		public int Status { get; set; }
	
		public static Photo getPhoto(int id)
		{
			Photo photo = Cache.Get(Cache.getCacheKey<Photo>(id)) as Photo;

			if (photo == null)
			{
				photo = new Photo(id);

				Cache.Set(Cache.getCacheKey<Photo>(id), photo);
			}

			return photo;
		}

        internal static void saveNewPhoto(int MemberID, string filename)
        {
            Cache.Remove(Cache.getCacheKey<Photo>(MemberID));

            using (var db = new DBmsw())
            {
                var targetPhoto = db.tPictures.SingleOrDefault(x => x.MemberID == MemberID);

                if (targetPhoto == null)
                {
                    targetPhoto = new tPicture();
                    db.tPictures.InsertOnSubmit(targetPhoto);
                    targetPhoto.MemberID = MemberID;
                }

                targetPhoto.NewPhotoFileName = filename;
                targetPhoto.Status = (int)PhotoStatus.UPLOADED;

                db.SubmitChanges();

                Photo photo = new Photo(targetPhoto);

                Cache.Set(Cache.getCacheKey<Photo>(MemberID), photo);
            }
        }

        public static void cropPhoto(int MemberID, string NewPhotoFileName, bool cropped)
        {
            Cache.Remove(Cache.getCacheKey<Photo>(MemberID));

            using (var db = new DBmsw())
            {
                var targetPhoto = db.tPictures.SingleOrDefault(x => x.MemberID == MemberID);
                targetPhoto.Status = (int)PhotoStatus.CROPPED;
                targetPhoto.NewPhotoFileName = NewPhotoFileName;

                db.SubmitChanges();

                Photo photo = new Photo(targetPhoto);

                Cache.Set(Cache.getCacheKey<Photo>(MemberID), photo);
            }
        }

        internal static void Moderate(int MemberID, bool approved)
        {
            Cache.Remove(Cache.getCacheKey<Photo>(MemberID));

			//Get User Information
			MSWUser user = MSWUser.getUser(MemberID);

            using (var db = new DBmsw())
            {
                var targetPhoto = db.tPictures.SingleOrDefault(x => x.MemberID == MemberID);

                if (approved)
                {
                    targetPhoto.FileName = targetPhoto.NewPhotoFileName;
                    targetPhoto.Status = (int)PhotoStatus.MODERATED;

                    //Picture has changed so the stewardship reports are expired
                    MSWtools.NukeStewardshipReports(user.WardStakeID.ToString());
                }
                else
                {
                    if (targetPhoto.FileName == null) // Sets photo to default
                    {
                        targetPhoto.Status = (int)PhotoStatus.NONE;
                    }
                    else //Sets old photo back to be the main photo for the member
                    {
                        targetPhoto.Status = (int)PhotoStatus.MODERATED;
                    }
                }

                targetPhoto.NewPhotoFileName = null;
                db.SubmitChanges();

                Photo photo = new Photo(targetPhoto);

                Cache.Set(Cache.getCacheKey<Photo>(MemberID), photo);
            }
        }

		private Photo(int MemberID)
		{
            using (var db = new DBmsw())
            {
                var photo = db.tPictures.SingleOrDefault(x => x.MemberID == MemberID);

                if (photo != null)
                {
                    this.MemberID = photo.MemberID;
                    FileName = photo.FileName == null ? "profile-1.jpg" : photo.FileName;
                    Status = photo.Status;
                    NewPhotoFileName = photo.NewPhotoFileName;
                }
                else
                {
                    this.MemberID = MemberID;
                    FileName = "profile-1.jpg";
                    Status = (int)PhotoStatus.NONE;
                }
            }
		}

		private Photo(tPicture photo)
		{
			if (photo != null)
			{
				MemberID = photo.MemberID;
				FileName = photo.FileName == null ? "profile-1.jpg" : photo.FileName;
				Status = photo.Status;
				NewPhotoFileName = photo.NewPhotoFileName;
			}
			else
			{
				this.MemberID = MemberID;
				FileName = "profile-1.jpg";
				Status = (int)PhotoStatus.NONE;
			}	
		
		}        
    }
}