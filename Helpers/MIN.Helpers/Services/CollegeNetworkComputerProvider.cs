using MIN.Helpers.Contracts.Interfaces;

namespace MIN.Helpers.Services;

/// <summary>
/// <see cref="ILocalNetworkComputerProvider"/> для колледжной компьютерной сети
/// </summary>
public class CollegeNetworkComputerProvider : ILocalNetworkComputerProvider
{
    private readonly int maxComputers = 20;

    string ILocalNetworkComputerProvider.GetLocalMachineName()
        => Environment.MachineName;

    IEnumerable<string> ILocalNetworkComputerProvider.GetLocalNetworkMachineNames(string searchZone)
    {
        var result = new List<string>();

        for (var i = 1; i <= maxComputers; i++)
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
