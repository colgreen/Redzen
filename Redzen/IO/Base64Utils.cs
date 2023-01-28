using System.Buffers;

namespace Redzen.IO;

/// <summary>
/// Base64 static utility methods.
/// </summary>
public static class Base64Utils
{
    /// <summary>
    /// Encodes a span of <see cref="Int32"/> to a base64 string, using a compact encoding.
    /// </summary>
    /// <param name="vals">The span of integers to convert.</param>
    /// <returns>A base64 string that encodes/represents the provided integers.</returns>
    /// <remarks>
    /// The integers are binary encoded using <see cref="BinaryWriter.Write7BitEncodedInt(int)"/>, which produces fewer
    /// encoding bytes for integer values that don't use the high bits. The binary encoding bytes are then base64
    /// encoded.
    /// </remarks>
    public static string ToBase64String(Span<int> vals)
    {
        // Binary encode the values.
        // Rent an array for the encoding bytes; we will need 5 bytes per integer in a worst case scenario,
        // so we allocate based on that worst case.
        // Notes.
        // We use a variable length encoding that uses 7 bits of each byte, and the high bit to indicate if a further
        // byte is required to be written. Therefore the worst case scenario occurs for integers with the highest bits
        // set, which would cause the variable length encoding to write 5 bytes per value. Finally, the number of values
        // being encoded is also written/encoded, and therefore we add one to span.Length.
        byte[] buff = ArrayPool<byte>.Shared.Rent((vals.Length + 1) * 5);

        try
        {
            // Create a memory stream wrapper around the pre-allocated buffer byte array.
            using var ms = new MemoryStream(buff);

            // Create a binary writer for writing into the memory stream.
            using var writer = new BinaryWriter(ms);

            // Write the number of values we will be writing to the stream.
            writer.Write7BitEncodedInt(vals.Length);

            // Loop the values; write each value into the buffer using a compact 7 bit binary encoding.
            foreach(int id in vals)
            {
                writer.Write7BitEncodedInt(id);
            }

            // Get a span over the written encoding bytes.
            var span = buff.AsSpan(0, (int)ms.Position);

            // Base64 encode the bytes to obtain a string representation.
            // Note. Base64 encoding increases the final number of encoded bytes by 33%, and therefore mostly cancels
            // out the benefit of the compact 7 bit encoding performed above.
            return Convert.ToBase64String(span);
        }
        finally
        {
            // Return the rented buffer array to the pool.
            ArrayPool<byte>.Shared.Return(buff);
        }
    }

    /// <summary>
    /// Decodes a base64 string produced by <see cref="ToBase64String(Span{int})"/> to an array of <see cref="Int32"/>.
    /// </summary>
    /// <param name="s">The base64 string containing the encoded integers/IDs.</param>
    /// <returns>A new array containing the decoded integers.</returns>
    public static int[] FromBase64String(string s)
    {
        // Rent a byte array with sufficient length to store all of the decoded base64 bytes.
        // Notes.
        // Base64 is written in blocks of 4 characters, with '=' padding characters to ensure the base64 string length
        // is always a multiple of 4. Each block of 4 chars represents 3 encoded bytes.
        byte[] buff = ArrayPool<byte>.Shared.Rent((s.Length / 4) * 3);

        try
        {
            // Decode the base64 characters into the buffer.
            if(!Convert.TryFromBase64String(s, buff, out int bytesWritten))
            {
                throw new ArgumentException("Invalid base64", nameof(s));
            }

            // Create a memory stream wrapper around the buffer byte array.
            using var ms = new MemoryStream(buff);

            // Create a binary reader for reading from the memory stream.
            using var reader = new BinaryReader(ms);

            // Read the number of integers that are encoded in the stream.
            var count = reader.Read7BitEncodedInt();

            // Read the integers, and append to a list.
            var arr = new int[count];

            for(int i = 0; i < count; i++)
            {
                arr[i] = reader.Read7BitEncodedInt();
            }

            return arr;
        }
        finally
        {
            // Return the rented buffer array to the pool.
            ArrayPool<byte>.Shared.Return(buff);
        }
    }
}
