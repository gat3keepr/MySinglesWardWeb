using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
    public class Note
    {
        public int NoteID { get; set; }
        public int MemberID { get; set; }
        public int CreatorID { get; set; }
		public string note { get; set; }		
        public bool isPublic { get; set; }

		public static Note get(int NoteID)
		{
			Note note = Cache.Get(Cache.getCacheKey<Note>(NoteID)) as Note;

			if (note == null)
			{
				note = new Note(NoteID);

				Cache.Set(Cache.getCacheKey<Note>(NoteID), note);
			}

			return note;
		}

		public static void save(Note note)
		{
			Cache.Remove(Cache.getCacheKey<Note>(note.NoteID));

            using (var db = new DBmsw())
            {
                var targetNote = db.tNotes.SingleOrDefault(x => x.NoteID == note.NoteID);

                if (targetNote == null)
                {
                    targetNote = new tNote();
                    targetNote.NoteID = note.NoteID;
                    db.tNotes.InsertOnSubmit(targetNote);
                }

                targetNote.MemberID = note.MemberID;
                targetNote.CreatorID = note.CreatorID;
                targetNote.Note = note.note;
                targetNote.isPublic = note.isPublic;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<Note>(note.NoteID), note);
            }
		}

		public static int create(tNote note)
		{
            using (var db = new DBmsw())
            {
                var targetNote = new tNote();
                db.tNotes.InsertOnSubmit(targetNote);

                targetNote.Note = note.Note;
                targetNote.MemberID = note.MemberID;
                targetNote.CreatorID = note.CreatorID;
                targetNote.isPublic = note.isPublic;

                db.SubmitChanges();
                Cache.Set(Cache.getCacheKey<Note>(targetNote.NoteID), new Note(targetNote.NoteID));
                return targetNote.NoteID;
            }
		}

		public static void remove(Note note)
		{
			Cache.Remove(Cache.getCacheKey<Note>(note.NoteID));

            using (var db = new DBmsw())
            {
                var targetNote = db.tNotes.SingleOrDefault(x => x.NoteID == note.NoteID);

                db.tNotes.DeleteOnSubmit(targetNote);
                db.SubmitChanges();
            }
		}

        private Note(int id)
		{
            using (var db = new DBmsw())
            {
                var dboNote = db.tNotes.SingleOrDefault(x => x.NoteID == id);

                this.NoteID = dboNote.NoteID;
                MemberID = dboNote.MemberID;
                CreatorID = dboNote.CreatorID;
                note = dboNote.Note;
                this.isPublic = dboNote.isPublic;
            }
		}
    }
}