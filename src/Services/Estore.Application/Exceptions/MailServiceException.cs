using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EStore.Application.Exceptions;

public class MailServiceException(string msg) : Exception(msg)
{
}
