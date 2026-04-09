using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace flow_desktop.UI.Services
{

    class FlowApiService
    {
        private readonly HttpClient _client;


        public FlowApiService()
        {
            _client = new HttpClient
            {
                // TODO replace with stable base url
                BaseAddress = new Uri("https://flow-api-o6gg.onrender.com")
            };

            

        }
    }
}
