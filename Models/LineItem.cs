namespace Models
{
    public class LineItem
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public decimal Cost { get; set; }
        public int Stay { get; set; }
        public System.DateTime StartDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is LineItem)) 
                return false;

            LineItem lineItem = obj as LineItem;
            return Id.Equals(lineItem.Id);
        }
    }
}