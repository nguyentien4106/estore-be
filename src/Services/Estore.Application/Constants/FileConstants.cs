using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estore.Application.Constants;

public static class FileConstants
{
    public static int FiveMBs = 5 * 1024 * 1024;
    public static int OneMB = 5 * 1024 * 1024;

    public static string GetFileName(string userName, string fileName) => $"{userName}/{fileName}".Replace(" ", "");
}
