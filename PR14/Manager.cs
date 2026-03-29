using System.Windows.Controls;

namespace PR14
{
    public static class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Users CurrentUser { get; set; }

        public static CinemaEntities GetContext()
        {
            return new CinemaEntities();
        }
    }
}