using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;

namespace monitoring.Services {
    public class MonitoringService {

        public IDictionary<string, double> limits;
        public IDictionary<string, int> counts;

        public IDictionary<string, string> colors;


        public MonitoringService()
        {
            limits = new Dictionary<string, double>();
            counts = new Dictionary<string, int>();
            colors = new Dictionary<string, string>();
            initDictionaries();
            
        }

        private void initDictionaries()
        {
            var colorNames = new List<String> {"orange", "blue", "purple", "red", "yellow"};
            var paramNames = new List<String>{"temp", "humidity", "smoke", "co", "lpg"};
            for(int i = 0; i < 5; i++)
            {
                limits.Add(paramNames[i], 0.0);
                counts.Add(paramNames[i], 0);
                colors.Add(paramNames[i], colorNames[i]);
            }
        }

    }
}