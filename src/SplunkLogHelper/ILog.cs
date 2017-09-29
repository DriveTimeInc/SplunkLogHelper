using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplunkLogHelper
{
    public interface ILog : IDisposable
    {
        void Message(string message, object payload = null);
        void Warning(string warning, object payload = null);
        void Error(string message, Exception ex, object payload = null);
    }
}
