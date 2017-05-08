using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLink.core
{
    public static class APIFactory
    {
        static APIInstance instance = null;
        public static APIInstance GetInstance()
        {
            if (instance == null)
            {
                string url = ConfigurationManager.AppSettings.Get("applink.api");
                instance = new APIInstance(url);
            }
            return instance;
        }
    }
}
