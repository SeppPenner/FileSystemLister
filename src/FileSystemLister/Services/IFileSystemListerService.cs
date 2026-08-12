// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystemListerService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The interface for the file system lister service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSystemLister.Services;

/// <summary>
/// The interface for the file system lister service.
/// </summary>
public interface IFileSystemListerService
{
    /// <summary>
    /// Lists the names of all files below the given directory, subdirectories included.
    /// </summary>
    /// <param name="directory">The directory to start at.</param>
    /// <param name="useBulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
    /// <returns>The file names as <see cref="IList{T}"/> of <see cref="string"/>.</returns>
    IList<string> ListFileNames(string directory, bool useBulletinCode);

    /// <summary>
    /// Writes the given lines to the result file, replacing an already existing file.
    /// </summary>
    /// <param name="fileName">The name of the file to write.</param>
    /// <param name="lines">The lines to write.</param>
    void WriteResultFile(string fileName, IEnumerable<string> lines);
}
