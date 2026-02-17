using MIN.Services.Contracts.Interfaces;

namespace MIN.Services.Services
{
    public class CollegeNetworkComputerProvider : ILocalNetworkComputerProvider
    {
        private readonly int maxComputers = 20;

        IEnumerable<string> ILocalNetworkComputerProvider.GetLocalNetworkComputerNames(string searchZone)
        {
            var result = new List<string>();

            for (int i = 1; i <= maxComputers; i++)
            {
                var pcName = CollegePCNameParser.CreateComputerName(Convert.ToInt32(searchZone), i);

                if (pcName == null)
                {
                    continue;
                }

                result.Add(pcName);
            }

            return result;
        }
    }
}
