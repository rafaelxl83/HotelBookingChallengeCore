using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class BookingContext
    {
        #region Singleton
        /// <summary>
        /// Application Response Singleton
        /// </summary>
        public static BookingContext Instance { get { return instance; } }
        private static readonly BookingContext instance = new BookingContext();
        private BookingContext() { }
        #endregion

        public List<LineItem> LineItems { get; set; } = new List<LineItem>();

        public bool Contains(object obj)
        {
            if (obj == null)
                return false;

            if (LineItems.Count == 0) 
                return false;

            LineItem item = obj as LineItem;
            return LineItems.Find(i => i.Id == item.Id) != null; 
        }
    }
}