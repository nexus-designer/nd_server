using NexusServer.Data;
using System.Text.RegularExpressions;

namespace NexusServer.Model
{
    public class DbHelper
    {
        private DataContext _context;
        public DbHelper(DataContext context)
        {
            _context = context;
        }
 
    }
}
