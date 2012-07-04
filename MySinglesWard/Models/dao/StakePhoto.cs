using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models.dbo
{
	[Serializable]
	public class StakePhoto
	{
		public int MemberID { get; set; }
		public string FileName { get; set; }
		public bool Cropped { get; set; }
		public string croppingFileName { get; set; }

		internal static void saveStakePhoto(int MemberID, string fileName)
		{
			Cache.Remove(Cache.getCacheKey<StakePhoto>(MemberID));
			Cache.Remove(Cache.getCacheKey<StakeUser>(MemberID));

            using (var db = new DBmsw())
            {
                tStakePhoto targetPhoto = db.tStakePhotos.SingleOrDefault(x => x.MemberID == MemberID);

                if (targetPhoto == null)
                {
                    targetPhoto = new tStakePhoto();
                    db.tStakePhotos.InsertOnSubmit(targetPhoto);
                    targetPhoto.MemberID = MemberID;
                }

                targetPhoto.FileName = fileName;
                targetPhoto.Cropped = false;

                //Needs to be cropped before HasPic becomes true
                tStakeUser user = db.tStakeUsers.SingleOrDefault(x => x.MemberID == MemberID);
                user.HasPic = false;

                db.SubmitChanges();

                StakePhoto photo = new StakePhoto(targetPhoto);

                Cache.Set(Cache.getCacheKey<StakePhoto>(MemberID), photo);
                Cache.Set(Cache.getCacheKey<StakeUser>(MemberID), user);
            }
		}

		public static void cropStakePhoto(int MemberID, string filename, bool cropped)
		{
			Cache.Remove(Cache.getCacheKey<StakePhoto>(MemberID));
			Cache.Remove(Cache.getCacheKey<StakeUser>(MemberID));

            using (var db = new DBmsw())
            {
                var targetPhoto = db.tStakePhotos.SingleOrDefault(x => x.MemberID == MemberID);
                targetPhoto.Cropped = cropped;
                targetPhoto.FileName = filename;

                var user = db.tStakeUsers.SingleOrDefault(x => x.MemberID == MemberID);
                user.HasPic = cropped;

                db.SubmitChanges();

                StakePhoto photo = new StakePhoto(targetPhoto);

                Cache.Set(Cache.getCacheKey<StakePhoto>(MemberID), photo);
                Cache.Set(Cache.getCacheKey<StakeUser>(MemberID), user);
            }
		}

		public static StakePhoto getStakePhoto(int id)
		{
			StakePhoto photo = Cache.Get(Cache.getCacheKey<StakePhoto>(id)) as StakePhoto;

			if (photo == null)
			{
				photo = new StakePhoto(id);
				Cache.Set(Cache.getCacheKey<StakePhoto>(id), photo);
			}

			return photo;
		}

		private StakePhoto(int MemberID)
		{
            using (var db = new DBmsw())
            {
                var photo = db.tStakePhotos.SingleOrDefault(x => x.MemberID == MemberID);

                if (photo != null)
                {
                    this.MemberID = photo.MemberID;
                    FileName = photo.FileName;
                    Cropped = photo.Cropped;
                }
                else
                {
                    this.MemberID = MemberID;
                    FileName = "stake-1.jpg";
                    Cropped = false;
                }
            }
		}

		private StakePhoto(tStakePhoto photo)
		{
			if (photo == null)
			{
				this.MemberID = photo.MemberID;
				FileName = "stake-1.jpg";
				Cropped = false;
			}
			else
			{				
				MemberID = photo.MemberID;
				FileName = photo.FileName;
				Cropped = photo.Cropped;

				if (!photo.Cropped)
				{
					FileName = "stake-1.jpg";
					croppingFileName = photo.FileName;
				}
			}
		}
	}
}