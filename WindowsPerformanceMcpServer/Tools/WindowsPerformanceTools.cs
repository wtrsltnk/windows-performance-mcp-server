using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Net.Http;

namespace SampleMcpServer.Tools;

public class WindowsPerformanceTools(
    IAppSettings config,
    HttpClient httpClient)
{
    /// <summary>
    /// Retrieves a table with performance statistics for the current machine.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [McpServerTool()]
    [Description("Retrieves a table with performance statistics for the current machine.")]
    public string CurrentPerformance()
    {
        return "Empty list";
    }
}