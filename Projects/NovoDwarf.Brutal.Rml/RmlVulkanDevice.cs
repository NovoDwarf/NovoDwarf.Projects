using Brutal.VulkanApi;
using Brutal.VulkanApi.Abstractions;

namespace NovoDwarf.Brutal.Rml;

/// <summary>
/// Borrowed graphics objects; the application owns their lifetime and synchronization.
/// </summary>
public readonly record struct RmlVulkanDevice(DeviceEx Device, PhysicalDevice PhysicalDevice, Queue Queue, int QueueFamily)
{
    internal static void Check(VkResult result, string operation)
    {
        if (result != VkResult.Success)
            throw new InvalidOperationException($"{operation} failed: {result}.");
    }
}
