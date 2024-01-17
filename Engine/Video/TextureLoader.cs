using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SharpDX;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Engine.Core;

namespace Engine.Video
{
    internal class TextureLoader
    {
        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SharpDX.WIC.BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, Stream stream)
        {

            var bitmapDecoder = new SharpDX.WIC.BitmapDecoder(factory, stream, System.Guid.NewGuid(), SharpDX.WIC.DecodeOptions.CacheOnDemand);

            var formatConverter = new SharpDX.WIC.FormatConverter(factory);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                SharpDX.WIC.BitmapDitherType.None,
                null,
                0.0,
                SharpDX.WIC.BitmapPaletteType.Custom);

            return formatConverter;
        }

        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SharpDX.WIC.BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, string filename)
        {

            var bitmapDecoder = new SharpDX.WIC.BitmapDecoder(
                factory,
                filename,
                SharpDX.WIC.DecodeOptions.CacheOnDemand
                );

            var formatConverter = new SharpDX.WIC.FormatConverter(factory);

            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                SharpDX.WIC.BitmapDitherType.None,
                null,
                0.0,
                SharpDX.WIC.BitmapPaletteType.Custom);

            return formatConverter;
        }

        public static ComPtr<ID3D11Texture2D> CreateTexture(ComPtr<ID3D11Device> device, Point size, Color4[] colors)
        {
            int bytesCount = colors.Length * 4;

            byte[] data = new byte[bytesCount];

            int pixel;

            for (int i = 0; i < bytesCount; i += 4)
            {
                pixel = i / 4;

                Color4 color = colors[pixel];

                data[i] = (byte)(int)(color.R * 255);
                data[i + 1] = (byte)(int)(color.G * 255);
                data[i + 2] = (byte)(int)(color.B * 255);
                data[i + 3] = (byte)(int)(color.A * 255);


            }

            return CreateTexture(device, size, data);
        }

        public unsafe static ComPtr<ID3D11Texture2D> CreateTexture(ComPtr<ID3D11Device> device, Point size, byte[] data)
        {
            ComPtr<ID3D11Texture2D> texture = default;

            var texture2DDesc = new Texture2DDesc()
            {
                Width = (uint)size.X,
                Height = (uint)size.Y,
                ArraySize = 1,
                BindFlags = (uint)BindFlag.ShaderResource,
                Usage = Usage.Default,
                CPUAccessFlags = (uint)CpuAccessFlag.None,
                Format = Format.FormatR8G8B8A8Unorm,
                MipLevels = 1,
                MiscFlags = (uint)ResourceMiscFlag.None,
                SampleDesc = new SampleDesc(1, 0),
            };

            fixed (byte* pData = data)
            {
                var subresourceData = new SubresourceData
                {
                    PSysMem = pData,
                    SysMemPitch = (uint)size.X * 4, // sizeof(uint32_t)
                    SysMemSlicePitch = 0,
                };

                CrashHandler.CheckForError(device.CreateTexture2D(texture2DDesc, subresourceData, ref texture), "Texture creation error");
            }

            return texture;

        }

        public static byte[] GetByteArrayFromBitmapSource(SharpDX.WIC.BitmapSource source)
        {
            
            int stride = source.Size.Width * 4;

            var buffer = new SharpDX.DataStream(source.Size.Height * stride, true, true);

            // Copy the content of the WIC to the buffer
            source.CopyPixels(stride, buffer);

            int byteSize = source.Size.Width * source.Size.Height * 4;

            byte[] data = new byte[byteSize];

            buffer.Read(data, 0, byteSize);

            return data;
        }

        public static byte[] GetByteArrayFromColors(Color[] colors)
        {
            byte[] data = new byte[4 * colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                Array.Copy(colors[i].Get(), 0, data, 4 * i, 4);
            }

            return data;
        }
    }

}
