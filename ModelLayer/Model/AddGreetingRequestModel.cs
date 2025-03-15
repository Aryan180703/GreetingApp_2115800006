using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Model
{
    public class AddGreetingRequestModel
    {
        public int UserID { get; set; }    
        public string Message { get; set; }
    }
}
