using MIN.Services.Contracts.Interfaces;

namespace MIN.Services.Services
{
    public class CollegeNetworkComputerProvider : ILocalNetworkComputerProvider
    {
        private readonly string hostPCname = string.Empty;
        private readonly int maxComputers = 20;

        public CollegeNetworkComputerProvider(string hostPCname)
        {
            this.hostPCname = hostPCname;
        }

        IEnumerable<string> ILocalNetworkComputerProvider.GetLocalNetworkComputerNames(string searchZone)
        {
            var result = new List<string>();

            for (int i = 0; i < maxComputers; i++)
            {
                var pcName = CollegePCNameParser.CreateComputerName(Convert.ToInt32(searchZone), i);

                if (pcName == null || pcName == hostPCname)
                {
                    continue;
                }

                result.Add(pcName);
            }

            return result;
        }
    }
}
