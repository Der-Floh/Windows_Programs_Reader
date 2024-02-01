namespace Windows_Programs_Reader.Service.EmbeddedResource;

public interface IEmbeddedResourceService
{
    /// <summary>
    /// Copies a resource from the embedded resources to the resources directory.
    /// </summary>
    /// <param name="name">The name of the resource to copy.</param>
    /// <returns>True if the resource is successfully copied, otherwise false.</returns>
    bool CopyResource(string name);

    /// <summary>
    /// Checks if a resource exists in the resources directory.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <returns>True if the resource exists, otherwise false.</returns>
    bool ExistsResource(string name);

    /// <summary>
    /// Gets the file path of the specified resource.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <returns>The file path of the resource.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the resource does not exist and cannot be copied from the embedded resources.</exception>
    string GetResourcePath(string name);

    public Stream GetResourceStream(string name);
}