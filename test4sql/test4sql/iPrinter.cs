using System;
using System.Collections.Generic;
using System.Text;

namespace test4sql
{
    public interface iPrinter
    {
        void Print(string ipAddress, int port, IList<string> lineToPrint);

    }
}
