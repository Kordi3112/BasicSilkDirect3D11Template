using Engine.Core;
using Silk.NET.Direct3D11;

namespace Engine.Video
{
    public class TextureFactory
    {

        public unsafe static byte[] GetByteDataFromTexture(VideoManager videoManager, Texture texture)
        {
        
            // Create staged
            var stagedTexture = Texture.CreateStaged(videoManager.Device, texture.Size);

            // Copy texture to staged
            videoManager.DeviceContext.CopyResource(stagedTexture, texture.GetTexture());

            var mappedSubresource = new MappedSubresource();

            CrashHandler.CheckForError(videoManager.DeviceContext.Map(stagedTexture, 0, Map.Read, 0, ref mappedSubresource), "TextureFactory.GetByteDataFromTexture: Unable to map resource");    

            int dataRowSize = 4 * texture.Size.X;

            byte[] data = new byte[dataRowSize * texture.Size.Y];

            // we must copy row by row
            fixed (byte* pData = data)
            {
                for (var i = 0; i < texture.Size.Y; i++)
                {
                    System.Buffer.MemoryCopy((byte*)mappedSubresource.PData + i * mappedSubresource.RowPitch, pData + i * dataRowSize, dataRowSize, dataRowSize);
                }
            }

            videoManager.DeviceContext.Unmap(stagedTexture, 0);

            stagedTexture.Dispose();

            return data;
        }

        /// <summary>
        /// All texture must have this same size
        /// </summary>
        public unsafe static byte[][] GetByteDataFromTextures(VideoManager videoManager, Texture[] textures)
        {
            if (textures is null || textures.Length < 1)
                return null;

            byte[][] data = new byte[textures.Length][];

            Vector2i size = textures[0].Size;

            int dataRowSize = 4 * size.X;

            // Create staged
            var stagedTexture = Texture.CreateStaged(videoManager.Device, textures[0].Size);

            for (int id = 0; id < textures.Length; id++)
            {
                data[id] = new byte[dataRowSize * size.Y];

                // Copy texture to staged
                videoManager.DeviceContext.CopyResource(stagedTexture, textures[id].GetTexture());

                var mappedSubresource = new MappedSubresource();

                CrashHandler.CheckForError(videoManager.DeviceContext.Map(stagedTexture, 0, Map.Read, 0, ref mappedSubresource), "TextureFactory.GetByteDataFromTexture: Unable to map resource");

                // we must copy row by row
                fixed (byte* pData = data[id])
                {
                    for (var i = 0; i < textures[0].Size.Y; i++)
                    {
                        System.Buffer.MemoryCopy((byte*)mappedSubresource.PData + i * mappedSubresource.RowPitch, pData + i * dataRowSize, dataRowSize, dataRowSize);
                    }
                }

                videoManager.DeviceContext.Unmap(stagedTexture, 0);
            }

            stagedTexture.Dispose();

            return data;
        }

        public unsafe static Color[] GetColorDataFromTexture(VideoManager videoManager, Texture texture)
        {
            byte[] bytes = GetByteDataFromTexture(videoManager, texture);

            var colors = new Color[bytes.Length / 4];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(bytes, i * 4);
            }

            return colors;
        }


        /// <summary>
        /// All texture must have this same size, returns Color[textureId][color]
        /// </summary>
        public unsafe static Color[][] GetColorDataFromTextures(VideoManager videoManager, Texture[] textures)
        {
            byte[][] bytes = GetByteDataFromTextures(videoManager, textures);

            if (bytes is null)
                return null;

            Color[][] colors = new Color[bytes.Length][];

            for (int id = 0; id < bytes.Length; id++)
            {
                colors[id] = new Color[bytes[id].Length / 4];

                for (int i = 0; i < colors[id].Length; i++)
                {
                    colors[id][i] = new Color(bytes[id], i * 4);
                }
            }


            return colors;
        }

    }
}
